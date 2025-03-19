using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using StockBackend.Services;
using StockBackend.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
            builder.Configuration["Jwt:Key"] ?? "defaultSecretKey12345678901234567890")),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoConnectionString = builder.Configuration.GetConnectionString("MongoDB") ?? "mongodb://localhost:27017";
var mongoClient = new MongoClient(mongoConnectionString);
var database = mongoClient.GetDatabase("stockdb");

builder.Services.AddSingleton<IMongoDatabase>(database);
builder.Services.AddScoped<IStockService, StockService>();

// Print startup message
Console.WriteLine("Starting up StockBackend application...");

var app = builder.Build();

app.UseCors(options =>
{
    options.WithOrigins("http://localhost:4200")
           .AllowCredentials()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add Authentication before Authorization
app.UseAuthentication(); 
app.UseAuthorization();

SeedData(database);

app.MapControllers();
Console.WriteLine("Controllers mapped successfully");

app.Run();

void SeedData(IMongoDatabase db)
{
    var stockCollection = db.GetCollection<Stock>("stocks");
    
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