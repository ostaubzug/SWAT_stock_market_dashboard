using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockBackend.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public List<string> SavedStocks { get; set; } = new();
} 