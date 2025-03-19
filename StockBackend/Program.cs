using MongoDB.Driver;
using StockBackend.Services;
using StockBackend.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure MongoDB
var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
var mongoClient = new MongoClient(mongoConnectionString);
var database = mongoClient.GetDatabase("stockdb");

// Register services
builder.Services.AddSingleton<IMongoDatabase>(database);
builder.Services.AddScoped<IStockService, StockService>();

var app = builder.Build();

// Configure CORS
app.UseCors(options =>
{
    options.WithOrigins("http://localhost:4200")
           .AllowCredentials()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Seed initial stock data
SeedData(database);

// Map controllers
app.MapControllers();

app.Run();

// Seed method to populate initial stock data
void SeedData(IMongoDatabase db)
{
    var stockCollection = db.GetCollection<Stock>("stocks");
    
    // Check if collection is empty
    if (!stockCollection.Find(_ => true).Any())
    {
        var stocksData = new List<Stock>
        {
            new Stock { Symbol = "AAPL", Name = "Apple Inc.", Price = 185.92m, Change = 0.87m },
            new Stock { Symbol = "MSFT", Name = "Microsoft Corporation", Price = 420.55m, Change = 1.25m },
            new Stock { Symbol = "GOOGL", Name = "Alphabet Inc.", Price = 175.98m, Change = -0.24m },
            new Stock { Symbol = "AMZN", Name = "Amazon.com, Inc.", Price = 178.75m, Change = 1.12m },
            new Stock { Symbol = "META", Name = "Meta Platforms, Inc.", Price = 502.30m, Change = 2.15m },
            new Stock { Symbol = "TSLA", Name = "Tesla, Inc.", Price = 177.67m, Change = -1.89m },
            new Stock { Symbol = "NVDA", Name = "NVIDIA Corporation", Price = 950.02m, Change = 5.34m },
            new Stock { Symbol = "JPM", Name = "JPMorgan Chase & Co.", Price = 198.87m, Change = 0.45m },
            new Stock { Symbol = "BAC", Name = "Bank of America Corporation", Price = 39.78m, Change = -0.22m },
            new Stock { Symbol = "V", Name = "Visa Inc.", Price = 279.56m, Change = 0.85m }
        };
        
        stockCollection.InsertMany(stocksData);
        Console.WriteLine("Database seeded with initial stock data.");
    }
}