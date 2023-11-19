using Microsoft.AspNetCore.SignalR;
using WebApplication.Models;

namespace WebApplication.Hubs;

public class SensorHub : Hub
{
    public async Task BroadcastSensorValue(SensorValue val)
    {
        await Clients.All.SendAsync("ReceiveMessage", val);
    }

    public void TestSignalR(string msg)
    {
        Console.WriteLine(msg);
    }
}