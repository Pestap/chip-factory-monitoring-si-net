using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;

namespace ChipFactorySimulator.Sensors;

public class TemperatureSensor : ISensor
{

    public string GetInfo()
    {
        return $"Temperature sensor (id={Id}), unit={UnitOfMeasurement}";
    }

    public IMqttClient MqttClient { get; set; }

    public TemperatureSensor(string name, double valueFrom, double valueTo, double interval, bool isRandom)
    {
        Name = name;
        Id = Guid.NewGuid();
        GeneratedValueRange = (from: valueFrom, to: valueTo);
        GeneratedSetValue = Convert.ToDouble(Environment.GetEnvironmentVariable("TEMP_SENSOR_VALUE_SINET"));
        UnitOfMeasurement = "K";
        Interval = interval;
        IsRandom = isRandom;
        Topic = $"sensors/temperature/{name}";
    }

    public bool IsRandom { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }

    public string UnitOfMeasurement { get; set; }
    public string Topic { get; set; }

    public (double from, double to) GeneratedValueRange { get; set; }
    public double Interval { get; set; }

    public double GeneratedSetValue { get; set; }

    

}