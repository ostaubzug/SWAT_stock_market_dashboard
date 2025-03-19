using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StockBackend.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    
    public string Name { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    
    public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
    
    public List<string> SavedStocks { get; set; } = new();
}

public class RegisterModel
{
    public string Name { get; set; } = string.Empty;
    
    public string FirstName { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
}

public class LoginModel
{
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
} 