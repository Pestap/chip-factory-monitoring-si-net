using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication.Models;

public class SensorValue
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Name { get; set; } = null!;
    public string UnitOfMeasurement { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public double Value { get; set; }
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime Time { get; set; }

    public SensorValue()
    {
        
    }
    public SensorValue(string? id, string name, string unitOfMeasurement, string topic, double value, DateTime time)
    {
        Id = id;
        Name = name;
        UnitOfMeasurement = unitOfMeasurement;
        Topic = topic;
        Value = value;
        Time = time;
    }
}