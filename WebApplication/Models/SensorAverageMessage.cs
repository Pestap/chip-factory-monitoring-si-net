namespace WebApplication.Models;

public class SensorAverageMessage
{
    public string Name { get; set; }
    public double AvgValue { get; set; }

    public string UnitOfMeasurement { get; set; }
    
    public SensorAverageMessage(string name, double avgValue, string unit)
    {
        this.Name = name;
        this.AvgValue = avgValue;
        this.UnitOfMeasurement = unit;
    }
}