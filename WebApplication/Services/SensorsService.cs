using System.Collections.Immutable;
using System.Globalization;
using Amazon.Runtime.Internal;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication.Models;

namespace WebApplication.Services;

public class SensorsService
{
    private readonly IMongoCollection<SensorValue> _sensorsValuesCollection;

    public SensorsService(IOptions<SensorsDatabaseSettings> sensorsDatabaseSettings)
    {
        var mongoClient = new MongoClient(sensorsDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(sensorsDatabaseSettings.Value.DatabaseName);

        _sensorsValuesCollection = mongoDatabase.GetCollection<SensorValue>(sensorsDatabaseSettings.Value.SensorValuesCollectionName);
    }

    public void Create(SensorValue newSensorValue)
    {
        _sensorsValuesCollection.InsertOne(newSensorValue);
    }


    public async Task<List<SensorValue>> GetAllAsync(string type, string name)
    {
        var builder = Builders<SensorValue>.Filter;
        var filter = builder.Empty;
        
        
        if (type != "")
        {
            filter &= builder.Eq(v => v.Topic, type);
        }

        if (name != "")
        {
            filter &= builder.Eq(v => v.Name, name);
        }

        filter &= builder.Lte(v => v.Time,DateTime.ParseExact("17-11-2023", "d-M-yyyy", CultureInfo.InvariantCulture));
        
        
        return await _sensorsValuesCollection.Find(filter).ToListAsync();
        
        
    }
}