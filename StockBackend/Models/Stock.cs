using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockBackend.Models;

public class Stock
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    public string Symbol { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Change { get; set; }
}

public class StockWithData : Stock
{
    public List<HistoricalData> Data { get; set; } = new();
}

public class HistoricalData
{
    public DateTime Date { get; set; }
    public decimal Price { get; set; }
} 