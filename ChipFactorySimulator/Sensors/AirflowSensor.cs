using MQTTnet.Client;

namespace ChipFactorySimulator.Sensors;

public class AirflowSensor : ISensor
{

    public string GetInfo()
    {
        return $"Airflow sensor (id={Id}), unit={UnitOfMeasurement}";
    }

    public IMqttClient MqttClient { get; set; }

    public AirflowSensor(string name, double valueFrom, double valueTo, double interval, bool isRandom)
    {
        Name = name;
        Id = Guid.NewGuid();
        GeneratedValueRange = (from: valueFrom, to: valueTo);
        GeneratedSetValue = Convert.ToDouble(Environment.GetEnvironmentVariable("AIRFLOW_SENSOR_VALUE_SINET"));
        UnitOfMeasurement = "m/s";
        Interval = interval;
        IsRandom = isRandom;
        Topic = $"sensors/airflow/{name}";
    }
    public (double from, double to) GeneratedValueRange { get; set; }
    public double Interval { get; set; }
    public double GeneratedSetValue { get; set; }
    public bool IsRandom { get; set; }
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UnitOfMeasurement { get; set; }
    public string Topic { get; set; }
}