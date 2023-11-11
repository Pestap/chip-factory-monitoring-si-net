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

        for (var i = 1; i < 4; i++)
        {
            await _mqttClient.SubscribeAsync($"sensors/humidity/h{i}");
            await _mqttClient.SubscribeAsync($"sensors/dust/d{i}");
            await _mqttClient.SubscribeAsync($"sensors/airflow/a{i}");
            await _mqttClient.SubscribeAsync($"sensors/temperature/t{i}");
        }
        _logger.LogInformation("subscribed");
    }
    public Task HandleApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        _logger.LogInformation("MESSAGE RECIEVED");
        var payload = System.Text.Encoding.Default.GetString(eventArgs.ApplicationMessage.PayloadSegment);
        var data = payload.Split(";");
        var topicParts = eventArgs.ApplicationMessage.Topic.Split("/");

        var topic = topicParts[1];
        var name = data[0];
        var value = double.Parse(data[1], System.Globalization.CultureInfo.InvariantCulture);
        var unitOfMeasurement = data[2];
        var time = Convert.ToDateTime(data[3]);
        Console.WriteLine($"{topic}-{name}-{value}-{unitOfMeasurement}-{time}");

        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            SensorsService sensorsService = scope.ServiceProvider.GetRequiredService<SensorsService>();

            SensorValue newSensorValue = new SensorValue(ObjectId.GenerateNewId().ToString(),
                name, unitOfMeasurement, topic, value);

            sensorsService.Create(newSensorValue);
        }
        
        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellingToken) 
    {
        /*
         TODO - Change hardcoded values for environmental variables later
        string user = Environment.GetEnvironmentVariable("MQTT_USER_SINET");
        string password = Environment.GetEnvironmentVariable("MQTT_PASSWORD_SINET");
        string mqtt_broker = Environment.GetEnvironmentVariable("MQTT_BROKER_SINET");
        int port = Convert.ToInt32(Environment.GetEnvironmentVariable("MQTT_PORT_SINET"));
        */
        string user = "admin";
        string password = "password";
        string mqtt_broker = "localhost";
        int port = 1883;
        
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