using System.Linq.Expressions;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices.JavaScript;
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
    private Dictionary<SensorsSortTypes, String> sortTypes;
    private readonly IHubContext<SensorHub> _hub;

    public SensorsService(IOptions<SensorsDatabaseSettings> sensorsDatabaseSettings, IHubContext<SensorHub> hub)
    {
        var mongoClient = new MongoClient(sensorsDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(sensorsDatabaseSettings.Value.DatabaseName);

        _sensorsValuesCollection = mongoDatabase.GetCollection<SensorValue>(sensorsDatabaseSettings.Value.SensorValuesCollectionName);
        
        sortTypes = new Dictionary<SensorsSortTypes, string>
        {
            { SensorsSortTypes.Default, "Default" },
            { SensorsSortTypes.SortByDateAsc, "Sort by date ascending" },
            { SensorsSortTypes.SortByDateDesc, "Sort by date descending" },
            { SensorsSortTypes.SortByNameAsc , "Sort by name ascending"},
            { SensorsSortTypes.SortByNameDesc, "Sort by name descending"},
            { SensorsSortTypes.SortByValueAsc, "Sort by value ascending"},
            { SensorsSortTypes.SortByValueDesc, "Sort by value descending"}
        };

        _hub = hub;

    }

    public void Create(SensorValue newSensorValue)
    {
        _sensorsValuesCollection.InsertOne(newSensorValue);
        _hub.Clients.All.SendAsync("SendSensor", newSensorValue.Name);
    }


    public async Task<List<SensorValue>> GetAllAsync(SensorsSortTypes sortType, string type, string name, string dateFrom, string dateTo)
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

        filter = typeFilter & nameFilter & dateFilter;
        
        if (type == "" && sortType != SensorsSortTypes.Default)
        {
            Expression<Func<SensorValue, object>> sortExpression = i => i.Value;
            var ascending = !(sortType == SensorsSortTypes.SortByDateDesc || sortType == SensorsSortTypes.SortByNameDesc ||
                               sortType == SensorsSortTypes.SortByValueDesc);
            switch (sortType)
            {
                case SensorsSortTypes.SortByDateDesc:
                case SensorsSortTypes.SortByDateAsc:
                    sortExpression = i => i.Time;
                    break;
                case SensorsSortTypes.SortByNameDesc:
                case SensorsSortTypes.SortByNameAsc:
                    sortExpression = i => i.Name;
                    break;
                case SensorsSortTypes.SortByValueDesc:
                case SensorsSortTypes.SortByValueAsc:
                    sortExpression = i => i.Value;
                    break;
            }
            
            if(ascending)
                return await _sensorsValuesCollection.Find( _ => true).SortBy(sortExpression).ToListAsync();
            return await _sensorsValuesCollection.Find( _ => true).SortByDescending(sortExpression).ToListAsync();
        }
        if (type == "" && sortType == SensorsSortTypes.Default)
        {
            return await _sensorsValuesCollection.Find( _ => true).ToListAsync();
        }
        return await _sensorsValuesCollection.Find(Builders<SensorValue>.Filter.Eq(v=> v.Topic, type)).ToListAsync();
    }

    public Dictionary<SensorsSortTypes, String> GetAllSortTypes()
    {
        return sortTypes;
    }
    
    public IEnumerable<String> GetAllNames()
    {
        ProjectionDefinition<SensorValue, String> projection = new FindExpressionProjectionDefinition<SensorValue, String>(v => v.Name);
        List<String> namesList = _sensorsValuesCollection.Find( _ => true).Project(projection).ToList();
        Console.WriteLine(namesList);
        namesList.Sort();
        return namesList.Distinct();
    }
    
    public IEnumerable<String> GetAllTypes()
    {
        ProjectionDefinition<SensorValue, String> projection = new FindExpressionProjectionDefinition<SensorValue, String>(v => v.Topic);
        List<String> typesList = _sensorsValuesCollection.Find( _ => true).Project(projection).ToList();
        typesList.Sort();
        return typesList.Distinct();
    }
}