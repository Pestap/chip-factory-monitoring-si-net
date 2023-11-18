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

        //filter &= dateFilter;



        if (sortBy != "" && sortDirection != "")
        {
            if (sortDirection == "asc")
            {
                return await _sensorsValuesCollection.Find(typeFilter & nameFilter & dateFilter).SortBy(e => e.Time).ToListAsync();
            }
            else
            {
                return await _sensorsValuesCollection.Find(typeFilter & nameFilter & dateFilter).SortByDescending(e => e.Time).ToListAsync();
            }
           
        }
        
        
        // no sort
        return await _sensorsValuesCollection.Find(typeFilter & nameFilter & dateFilter).ToListAsync();
        
        
    }
}