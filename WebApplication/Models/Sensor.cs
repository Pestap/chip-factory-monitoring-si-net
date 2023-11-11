using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models;

public class Sensor
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public string UnitOfMeasurement { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public double Value { get; set; }
}