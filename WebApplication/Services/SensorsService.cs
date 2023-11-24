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
    private Dictionary<string, List<SensorValue>> sensorValues;
    private readonly IHubContext<SensorHub> _hub;
    private readonly ILogger<SensorsService> _logger;

    public SensorsService(ILogger<SensorsService> logger,IOptions<SensorsDatabaseSettings> sensorsDatabaseSettings, IHubContext<SensorHub> hub)
    {
        _logger = logger;
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
        
        Console.WriteLine("TEST INIT");
        sensorValues = new Dictionary<string, List<SensorValue>>();
        _hub = hub;

    }
        
    private double CalculateAverageOfSensor(string sensorName)
    {   
        if (!sensorValues.ContainsKey(sensorName))
        {
            Console.WriteLine("INVALID NAME");
            throw new ArgumentException("Invalid sensor name");
        }

        if (!sensorValues[sensorName].Any())
        {
            return 0;
        }

        return sensorValues[sensorName].Take(100).Select(item => item.Value).Average();

    }
    
    public void Create(SensorValue newSensorValue)
    {
        if (!sensorValues.ContainsKey(newSensorValue.Name))
        {
            sensorValues[newSensorValue.Name] = new List<SensorValue>();
        }
        sensorValues[newSensorValue.Name].Add(newSensorValue); ;
        
        _sensorsValuesCollection.InsertOne(newSensorValue);
        
        _hub.Clients.All.SendAsync("SendSensorValue", $"{newSensorValue.Name},{newSensorValue.Value},{newSensorValue.UnitOfMeasurement}");
        _hub.Clients.All.SendAsync("SendAverageValue", $"{newSensorValue.Name},{CalculateAverageOfSensor(newSensorValue.Name)},{newSensorValue.UnitOfMeasurement}");
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
        
        Expression<Func<SensorValue, object>> sortExpression = i => i.Value;
        var ascending = !(sortType == SensorsSortTypes.SortByDateDesc || sortType == SensorsSortTypes.SortByNameDesc ||
                           sortType == SensorsSortTypes.SortByValueDesc);
        Console.WriteLine(sortType);
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
            case SensorsSortTypes.Default:
                return await _sensorsValuesCollection.Find(filter).ToListAsync();
        }
            
        if(ascending)
            return await _sensorsValuesCollection.Find( filter).SortBy(sortExpression).ToListAsync();
        return await _sensorsValuesCollection.Find(filter).SortByDescending(sortExpression).ToListAsync();

        
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