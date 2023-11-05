using MQTTnet.Client;

namespace ChipFactorySimulator.Sensors;

public class DustSensor : ISensor
{

    public string GetInfo()
    {
        return $"Dust sensor (id={Id}), unit={UnitOfMeasurement}";
    }

    public IMqttClient MqttClient { get; set; }

    public DustSensor(string name, double valueFrom, double valueTo, double interval)
    {
        Name = name;
        Id = Guid.NewGuid();
        GeneratedValueRange = (from: valueFrom, to: valueTo);
        GeneratedSetValue = Convert.ToDouble(Environment.GetEnvironmentVariable("HUMIDITY_SENSOR_VALUE_SINET"));
        UnitOfMeasurement = "µg/m3";
        Interval = interval;
        Topic = $"sensors/dust/{name}";
        
    }
    public (double from, double to) GeneratedValueRange { get; set; }
    public double Interval { get; set; }
    public double GeneratedSetValue { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UnitOfMeasurement { get; set; }
    public string Topic { get; set; }
}