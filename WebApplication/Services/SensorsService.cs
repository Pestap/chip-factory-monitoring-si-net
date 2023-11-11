using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication.Models;

namespace WebApplication.Services;

public class SensorsService: BackgroundService // TODO - REMOVE BackgroundService INTERFACE AND MAKE SensorsService SCOPED IN Program.cs
{
    private readonly IMongoCollection<Sensor> _sensorsValuesCollection;

    public SensorsService(IOptions<SensorsDatabaseSettings> sensorsDatabaseSettings)
    {
        var mongoClient = new MongoClient(sensorsDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(sensorsDatabaseSettings.Value.DatabaseName);

        _sensorsValuesCollection = mongoDatabase.GetCollection<Sensor>(sensorsDatabaseSettings.Value.SensorValuesCollectionName);
    }
    
    // TODO - REMOVE THIS FUNCTION
    protected override async Task ExecuteAsync(CancellationToken cancellingToken)
    {
        Sensor firstSensorValue = new Sensor();
        firstSensorValue.Id = ObjectId.GenerateNewId().ToString();
        firstSensorValue.Value = 230.34;
        firstSensorValue.Name = "das";
        firstSensorValue.Topic = "TWTWTWT";
        firstSensorValue.UnitOfMeasurement = "KM/H";
        await _sensorsValuesCollection.InsertOneAsync(firstSensorValue);
        while(!cancellingToken.IsCancellationRequested) 
        { 
            //Console.WriteLine("Working behind the scenes..."); 
            await Task.Delay(5000, cancellingToken); 
        } 
    }
}