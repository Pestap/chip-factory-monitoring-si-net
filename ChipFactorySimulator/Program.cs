// See https://aka.ms/new-console-template for more information

using System.Security.AccessControl;
using ChipFactorySimulator.Sensors;
using System;
using ChipFactorySimulator.Utils;
using MQTTnet;
using MQTTnet.Client;


string user = Environment.GetEnvironmentVariable("MQTT_USER_SINET");
string password = Environment.GetEnvironmentVariable("MQTT_PASSWORD_SINET");
string mqtt_broker = Environment.GetEnvironmentVariable("MQTT_BROKER_SINET");
int port = Convert.ToInt32(Environment.GetEnvironmentVariable("MQTT_PORT_SINET"));


Console.WriteLine($"{user} - {password}");
Console.WriteLine($"{mqtt_broker} - broker");
Console.WriteLine($"{port} - port");

var factory = new MqttFactory();

// Create a MQTT client instance
var mqttClient = factory.CreateMqttClient();

// Create MQTT client options
var options = new MqttClientOptionsBuilder()
    .WithTcpServer(mqtt_broker, port) // MQTT broker address and port
    .WithCredentials(user, password) // Set username and password
    .WithClientId("Generator")
    .WithCleanSession()
    .Build();

var connectResult = await mqttClient.ConnectAsync(options);


List<ISensor> sensors = new List<ISensor>();
sensors = FileUtils.ReadSensorsFromFile("/generator_config/sensors.csv");

foreach (var sensor in sensors)
{
    sensor.MqttClient = mqttClient;
}



var threads = new List<Thread>();
foreach (ISensor sensor in sensors)
{
    Thread t = new Thread(sensor.StartGenerating);
    threads.Add(t);
    t.Start();
}

foreach (var t in threads)
{
    t.Join();
    Console.WriteLine($"Thread finished");
}

Console.WriteLine("Generator finished");









