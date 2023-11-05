using MQTTnet.Client;

namespace ChipFactorySimulator.Sensors;

public class HumiditySensor : ISensor
{
    
    public string GetInfo()
    {
        return $"Humidity sensor (id={Id}), unit={UnitOfMeasurement}";
    }

    public IMqttClient MqttClient { get; set; }

    public HumiditySensor(string name, double valueFrom, double valueTo, double interval)
    {
        Name = name;
        Id = Guid.NewGuid();
        GeneratedValueRange = (from: valueFrom, to: valueTo);
        GeneratedSetValue = Convert.ToDouble(Environment.GetEnvironmentVariable("HUMIDITY_SENSOR_VALUE_SINET"));
        UnitOfMeasurement = "%";
        Interval = interval;
        Topic = $"sensors/humidity/{name}";
    }
    public (double from, double to) GeneratedValueRange { get; set; }
    public double Interval { get; set; }
    public double GeneratedSetValue { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UnitOfMeasurement { get; set; }
    public string Topic { get; set; }
}