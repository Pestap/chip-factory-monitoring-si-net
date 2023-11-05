namespace ChipFactorySimulator.Sensors;

public class HumiditySensor : ISensor
{
    public void PublishData()
    {
        throw new NotImplementedException();
    }
    
    public string GetInfo()
    {
        return $"Humidity sensor (id={Id}), unit={UnitOfMeasurement}";
    }

    public HumiditySensor(string name, double valueFrom, double valueTo, double interval)
    {
        Name = name;
        Id = Guid.NewGuid();
        GeneratedValueRange = (from: valueFrom, to: valueTo);
        GeneratedSetValue = Convert.ToDouble(Environment.GetEnvironmentVariable("HUMIDITY_SENSOR_VALUE_SINET"));
        UnitOfMeasurement = "%";
        Interval = interval;
    }
    public (double from, double to) GeneratedValueRange { get; set; }
    public double Interval { get; set; }
    public double GeneratedSetValue { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UnitOfMeasurement { get; set; }
}