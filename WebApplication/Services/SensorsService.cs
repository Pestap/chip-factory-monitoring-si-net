using System.Collections.Immutable;
using System.Globalization;
using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication.Hubs;
using WebApplication.Models;

namespace WebApplication.Services;

public class SensorsService
{
    private readonly IMongoCollection<SensorValue> _sensorsValuesCollection;
    private readonly IHubContext<SensorHub> _hub;

    public SensorsService(IOptions<SensorsDatabaseSettings> sensorsDatabaseSettings, IHubContext<SensorHub> hub)
    {
        var mongoClient = new MongoClient(sensorsDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(sensorsDatabaseSettings.Value.DatabaseName);

        _sensorsValuesCollection = mongoDatabase.GetCollection<SensorValue>(sensorsDatabaseSettings.Value.SensorValuesCollectionName);

        _hub = hub;

    }

    public void Create(SensorValue newSensorValue)
    {
        _sensorsValuesCollection.InsertOne(newSensorValue);
        _hub.Clients.All.SendAsync("SendSensor", newSensorValue.Name);
    }


    public async Task<List<SensorValue>> GetAllAsync(string type, string name, string dateFrom, string dateTo, string sortBy, string sortDirection)
    {
        var builder = Builders<SensorValue>.Filter;
        var filter = builder.Empty;
        
        
        
        // type filtering

        var typeFilter = builder.Empty;
        if (type != "")
        {
            typeFilter &= builder.In(v => v.Topic, type.Split(","));
        }

        

        var nameFilter = builder.Empty;
        if (name != "")
        {
            nameFilter &= builder.In(v => v.Name, name.Split(","));
        }
        
        // date filter
        
        var dateFilter = builder.Empty;

        if (dateFrom != "")
        {
            var dateFromDate = DateTime.ParseExact(dateFrom, "yyyy-M-d'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
            dateFilter &= builder.Gte(v => v.Time, dateFromDate);
            

        }

        if (dateTo != "")
        {
            var dateToDate = DateTime.ParseExact(dateTo, "yyyy-M-d'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture);
            dateFilter &= builder.Lte(v => v.Time, dateToDate);
        }
        
        // sort
        if (sortBy != "" && sortDirection != "")
        {
            switch (sortBy)
            {
                case "type":
                    switch (sortDirection)
                    {
                        case "asc":
                            return await _sensorsValuesCollection.Find(typeFilter & nameFilter & dateFilter).SortBy(e => e.Topic).ToListAsync();
                        case "dsc":
                            return await _sensorsValuesCollection.Find(typeFilter & nameFilter & dateFilter).SortByDescending(e => e.Topic).ToListAsync();
                    }
                    break;
                case "time":
                    switch (sortDirection)
                    {
                        case "asc":
                            return await _sensorsValuesCollection.Find(typeFilter & nameFilter & dateFilter).SortBy(e => e.Time).ToListAsync();
                        case "dsc":
                            return await _sensorsValuesCollection.Find(typeFilter & nameFilter & dateFilter).SortByDescending(e => e.Time).ToListAsync();
                    }
                    break;
                case "name":
                    switch (sortDirection)
                    {
                        case "asc":
                            return await _sensorsValuesCollection.Find(typeFilter & nameFilter & dateFilter).SortBy(e => e.Name).ToListAsync();
                        case "dsc":
                            return await _sensorsValuesCollection.Find(typeFilter & nameFilter & dateFilter).SortByDescending(e => e.Name).ToListAsync();
                    }
                    break;
            }
        }
        


        // no sort
        return await _sensorsValuesCollection.Find(typeFilter & nameFilter & dateFilter).ToListAsync();
        
        
    }
}