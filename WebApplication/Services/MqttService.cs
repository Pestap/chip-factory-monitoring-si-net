using System.Globalization;
using MongoDB.Bson;
using WebApplication.Models;

namespace WebApplication.Services;
using MQTTnet;
using MQTTnet.Client;

public class MqttService : BackgroundService 
{
    private readonly ILogger<MqttService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IMqttClient _mqttClient;
    
    public MqttService(ILogger<MqttService> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        
        var factory = new MqttFactory();
        // Create a MQTT client instance
        _mqttClient = factory.CreateMqttClient();
        _mqttClient.ConnectedAsync += HandleConnectedAsync;
        _mqttClient.ApplicationMessageReceivedAsync += HandleApplicationMessageReceivedAsync;
    }
    
    public async Task HandleConnectedAsync(MqttClientConnectedEventArgs eventArgs)
    {
        _logger.LogInformation("connected - subscribing");

        await _mqttClient.SubscribeAsync($"sensors/humidity/#");
        await _mqttClient.SubscribeAsync($"sensors/dust/#");
        await _mqttClient.SubscribeAsync($"sensors/airflow/#");
        await _mqttClient.SubscribeAsync($"sensors/temperature/#");
        
        _logger.LogInformation("subscribed");
    }
    public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        _logger.LogInformation("MESSAGE RECEIVED");
        var payload = System.Text.Encoding.Default.GetString(eventArgs.ApplicationMessage.PayloadSegment);
        var data = payload.Split(";");
        var topicParts = eventArgs.ApplicationMessage.Topic.Split("/");
        var topic = topicParts[1];
        var name = data[0];
        var value = double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture);
        var unitOfMeasurement = data[2];
        var time = Convert.ToDateTime(data[3], new DateTimeFormatInfo());
        Console.WriteLine($"{topic}-{name}-{value}-{unitOfMeasurement}-{time}");

        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            SensorsService sensorsService = scope.ServiceProvider.GetRequiredService<SensorsService>();

            SensorValue newSensorValue = new SensorValue(ObjectId.GenerateNewId().ToString(),
                name, unitOfMeasurement, topic, value, time);

            sensorsService.Create(newSensorValue);
        }
        
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellingToken) 
    {
        // TODO - Comment these 4 lines later
        Environment.SetEnvironmentVariable("MQTT_USER_SINET", "admin");
        Environment.SetEnvironmentVariable("MQTT_PASSWORD_SINET", "password");
        Environment.SetEnvironmentVariable("MQTT_BROKER_SINET", "localhost");
        Environment.SetEnvironmentVariable("MQTT_PORT_SINET", "1883");
        
        string user = Environment.GetEnvironmentVariable("MQTT_USER_SINET");
        string password = Environment.GetEnvironmentVariable("MQTT_PASSWORD_SINET");
        string mqtt_broker = Environment.GetEnvironmentVariable("MQTT_BROKER_SINET");
        int port = Convert.ToInt32(Environment.GetEnvironmentVariable("MQTT_PORT_SINET"));
        
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(mqtt_broker, port) // MQTT broker address and port
            .WithCredentials(user, password) // Set username and password
            .WithClientId("Client")
            .WithCleanSession()
            .Build();
        var connectResult = await _mqttClient.ConnectAsync(options);
        _logger.LogInformation("Connected " + connectResult.ResponseInformation);
        
        while(!cancellingToken.IsCancellationRequested) 
        { 
            //Console.WriteLine("Working behind the scenes..."); 
            await Task.Delay(5000, cancellingToken); 
        } 
    }
    
}