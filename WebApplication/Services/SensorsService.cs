﻿using Microsoft.Extensions.Options;
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


    public async Task<List<SensorValue>> GetAllAsync(string type)
    {
        if (type == "")
        {
            return await _sensorsValuesCollection.Find( _ => true).ToListAsync();
        }
        return await _sensorsValuesCollection.Find(Builders<SensorValue>.Filter.Eq(v=> v.Topic, type)).ToListAsync();
    }
}