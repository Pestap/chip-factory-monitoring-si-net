using WebApplication.Hubs;
using WebApplication.Models;
using WebApplication.Services;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddHostedService<MqttService>();
builder.Services.Configure<SensorsDatabaseSettings>(builder.Configuration.GetSection("SensorsDatabase"));
builder.Services.AddScoped<SensorsService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin();
        });
});
builder.Services.AddSignalR(options =>
{
    options.HandshakeTimeout = TimeSpan.FromSeconds(5);
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.EnableDetailedErrors = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");
app.MapHub<SensorHub>("/sensorhub");

app.Run();