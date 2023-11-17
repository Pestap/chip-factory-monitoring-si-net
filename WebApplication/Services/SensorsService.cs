using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using WebApplication.Models;

namespace WebApplication.Services;

public class SensorsService
{
    private readonly IMongoCollection<SensorValue> _sensorsValuesCollection;
    private Dictionary<SensorsSortTypes, String> sortTypes;

    public SensorsService(IOptions<SensorsDatabaseSettings> sensorsDatabaseSettings)
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
    }

    public void Create(SensorValue newSensorValue)
    {
        _sensorsValuesCollection.InsertOne(newSensorValue);
    }


    public async Task<List<SensorValue>> GetAllAsync(SensorsSortTypes sortType, string type)
    {
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
}