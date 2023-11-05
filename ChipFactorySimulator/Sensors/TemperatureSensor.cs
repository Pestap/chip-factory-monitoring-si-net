namespace ChipFactorySimulator.Sensors;

public class TemperatureSensor : ISensor
{
    public void PublishData()
    {
        throw new NotImplementedException();
    }

    public string GetInfo()
    {
        return $"Temperature sensor (id={Id}), unit={UnitOfMeasurement}";
    }

    public TemperatureSensor(string name, double valueFrom, double valueTo, double interval)
    {
        Name = name;
        Id = Guid.NewGuid();
        GeneratedValueRange = (from: valueFrom, to: valueTo);
        GeneratedSetValue = Convert.ToDouble(Environment.GetEnvironmentVariable("TEMP_SENSOR_VALUE_SINET"));
        UnitOfMeasurement = "K";
        Interval = interval;
    }
    
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string UnitOfMeasurement { get; set; }

    public (double from, double to) GeneratedValueRange { get; set; }
    public double Interval { get; set; }

    public double GeneratedSetValue { get; set; }

    

}