using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace ChipFactorySimulator.Sensors;

public interface ISensor
{
    public void PublishData(double data, DateTime time)
    {

        MqttClient.PublishAsync(new MqttApplicationMessageBuilder()
            .WithPayload($"{Name};{data};{UnitOfMeasurement};{time}")
            .WithTopic(Topic)
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
            .Build());
    }
    string GetInfo();
    IMqttClient MqttClient { get; set; }

    double GenerateDataPoint()
    {
        return IsRandom ? GenerateRandomDataPoint() : GenerateSetDataPoint();
    }

    double GenerateRandomDataPoint()
    {
        var random = new Random();
        return random.NextDouble() * (GeneratedValueRange.from - GeneratedValueRange.to) + GeneratedValueRange.to;
    }

    double GenerateSetDataPoint()
    {
        return GeneratedSetValue;
    }
    
    void StartGenerating()
    {
        int i = 0;
        while (i++< Convert.ToInt64(Environment.GetEnvironmentVariable("SINET_POINTS_PER_SENSOR")))
        {
            PublishData(GenerateDataPoint(), DateTime.Now);
            Thread.Sleep((int)(60000/Interval));
            
        }
    }

    (double from, double to) GeneratedValueRange { get; set; }
    double Interval { get; set; }

    double GeneratedSetValue { get; set; }
    
    bool IsRandom { get; set; }

    Guid Id { get; set; }

    string Name { get; set; }
    string UnitOfMeasurement
    {
        get;
        set;
    }
    
    string Topic { get; set; }
}