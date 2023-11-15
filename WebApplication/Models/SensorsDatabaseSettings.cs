namespace WebApplication.Models;

public class SensorsDatabaseSettings
{
    public string ConnectionString
    {
        get { return Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");}
        set { Environment.SetEnvironmentVariable("MONGO_CONNECTION_STRING", value);}
    }

    public string DatabaseName { get; set; } = null!;

    public string SensorValuesCollectionName { get; set; } = null!;
}