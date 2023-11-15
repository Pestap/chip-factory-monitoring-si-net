namespace WebApplication.Models;

public class SensorsDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string SensorValuesCollectionName { get; set; } = null!;
}