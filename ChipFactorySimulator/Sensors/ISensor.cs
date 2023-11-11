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

    double GenerateDataPoint(bool random=true)
    {
        return random ? GenerateRandomDataPoint() : GenerateSetDataPoint();
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
        while (true)
        {
            PublishData(GenerateDataPoint(), DateTime.Now);
            Thread.Sleep((int)(600000/Interval));
        }
    }

    (double from, double to) GeneratedValueRange { get; set; }
    double Interval { get; set; }

    double GeneratedSetValue { get; set; }

    Guid Id { get; set; }

    string Name { get; set; }
    string UnitOfMeasurement
    {
        get;
        set;
    }
    
    string Topic { get; set; }
}