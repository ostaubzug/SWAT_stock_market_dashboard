using MongoDB.Driver;
using StockBackend.Models;

namespace StockBackend.Services;

public interface IStockService
{
    Task<List<Stock>> GetAllStocksAsync();
    Task<Stock?> GetStockBySymbolAsync(string symbol);
    Task<List<StockWithData>> GetUserStocksAsync(List<string> symbols);
}

public class StockService : IStockService
{
    private readonly IMongoCollection<Stock> _stocks;
    private readonly Random _random = new();

    public StockService(IMongoDatabase database)
    {
        _stocks = database.GetCollection<Stock>("stocks");
    }

    public async Task<List<Stock>> GetAllStocksAsync()
    {
        return await _stocks.Find(_ => true).ToListAsync();
    }

    public async Task<Stock?> GetStockBySymbolAsync(string symbol)
    {
        return await _stocks.Find(s => s.Symbol == symbol).FirstOrDefaultAsync();
    }

    public async Task<List<StockWithData>> GetUserStocksAsync(List<string> symbols)
    {
        var stocks = new List<StockWithData>();
        
        foreach (var symbol in symbols)
        {
            var stock = await GetStockBySymbolAsync(symbol);
            if (stock != null)
            {
                stocks.Add(new StockWithData
                {
                    Id = stock.Id,
                    Symbol = stock.Symbol,
                    Name = stock.Name,
                    Price = stock.Price,
                    Change = stock.Change,
                    Data = GenerateMockHistoricalData(symbol)
                });
            }
        }

        return stocks;
    }

    private List<HistoricalData> GenerateMockHistoricalData(string symbol)
    {
        var data = new List<HistoricalData>();
        var basePrice = _random.Next(50, 200);
        var currentDate = DateTime.Now.AddDays(-30);

        for (int i = 0; i < 30; i++)
        {
            data.Add(new HistoricalData
            {
                Date = currentDate.AddDays(i),
                Price = basePrice + _random.Next(-20, 20)
            });
        }

        return data;
    }
} 