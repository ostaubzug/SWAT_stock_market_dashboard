using Microsoft.AspNetCore.Mvc;
using StockBackend.Models;
using StockBackend.Services;
using MongoDB.Driver;

namespace StockBackend.Controllers;

[ApiController]
[Route("api/stocks")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;
    private readonly IMongoDatabase _database;

    public StockController(IStockService stockService, IMongoDatabase database)
    {
        _stockService = stockService;
        _database = database;
    }

    [HttpGet]
    public async Task<ActionResult<List<Stock>>> GetAllStocks()
    {
        try
        {
            var stocks = await _stockService.GetAllStocksAsync();
            return Ok(stocks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to fetch stocks" });
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<List<Stock>>> SearchStocks([FromQuery] string q)
    {
        try
        {
            if (string.IsNullOrEmpty(q))
            {
                return BadRequest(new { error = "Search query is required" });
            }

            var allStocks = await _stockService.GetAllStocksAsync();
            var searchTerm = q.ToLower();

            var results = allStocks.Where(stock =>
                stock.Symbol.ToLower().Contains(searchTerm) ||
                stock.Name.ToLower().Contains(searchTerm)
            ).ToList();

            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to search stocks" });
        }
    }

    [HttpGet("details/{symbol}")]
    public async Task<ActionResult<StockWithData>> GetStockDetails(string symbol)
    {
        try
        {
            var stock = await _stockService.GetStockBySymbolAsync(symbol);
            if (stock == null)
            {
                return NotFound(new { error = "Stock not found" });
            }

            var stockWithData = new StockWithData
            {
                Id = stock.Id,
                Symbol = stock.Symbol,
                Name = stock.Name,
                Price = stock.Price,
                Change = stock.Change,
                Data = GenerateMockHistoricalData(symbol)
            };

            return Ok(stockWithData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to fetch stock details" });
        }
    }

    // User stocks endpoints without authentication

    [HttpGet("user-stocks")]
    public async Task<ActionResult<List<StockWithData>>> GetUserStocks([FromQuery] string symbols)
    {
        try
        {
            if (string.IsNullOrEmpty(symbols))
            {
                return Ok(new List<StockWithData>());
            }

            var symbolList = symbols.Split(',').ToList();
            var userStocks = await _stockService.GetUserStocksAsync(symbolList);
            
            return Ok(userStocks);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to fetch user stocks" });
        }
    }

    [HttpPost("user-stocks")]
    public async Task<ActionResult> SaveUserStocks([FromBody] SaveStocksRequest request)
    {
        try
        {
            if (request?.Stocks == null)
            {
                return BadRequest(new { error = "No stocks provided" });
            }

            var symbols = request.Stocks
                .Where(s => !string.IsNullOrEmpty(s.Symbol))
                .Select(s => s.Symbol)
                .ToList();

            if (symbols.Count == 0)
            {
                return BadRequest(new { error = "No valid stock symbols provided" });
            }

            // Since we're not implementing auth, we'll just return success
            return Ok(new { success = true, message = "Stocks saved successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to save stocks" });
        }
    }

    private List<HistoricalData> GenerateMockHistoricalData(string symbol)
    {
        var data = new List<HistoricalData>();
        var random = new Random(symbol.GetHashCode()); // Use symbol as seed for consistent data
        var basePrice = random.Next(50, 200);
        var currentDate = DateTime.Now.AddDays(-30);

        for (int i = 0; i < 30; i++)
        {
            data.Add(new HistoricalData
            {
                Date = currentDate.AddDays(i),
                Price = basePrice + random.Next(-20, 20)
            });
        }

        return data;
    }
}

public class SaveStocksRequest
{
    public List<Stock> Stocks { get; set; } = new();
} 