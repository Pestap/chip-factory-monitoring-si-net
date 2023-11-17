using System.Text;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.Services;

namespace WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly SensorsService _sensorsService;

    public SensorsController(SensorsService sensorsService)
    {
        _sensorsService = sensorsService;
    }

    [HttpGet]
    public async Task<List<SensorValue>> GetAllSensors([FromQuery(Name = "sort-by")] SensorsSortTypes sortType, string type="")
    {
        return await _sensorsService.GetAllAsync(sortType, type);
    }
    
    [HttpGet]
    [Route("sort-types")]
    public Dictionary<SensorsSortTypes, String> GetAllSortTypes()
    {
        return _sensorsService.GetAllSortTypes();
    }

    [HttpGet]
	[Route("json")]
    public IActionResult GetAllSensorsJson([FromQuery(Name = "sort-by")] SensorsSortTypes sortType, string type="")
    {
        HttpContext.Response.Headers.Add("Content-Disposition", "attachment; filename=values.json");
        return new JsonResult(_sensorsService.GetAllAsync(sortType, type).Result);
    }
    
    [HttpGet]
    [Route("csv")]
    public IActionResult GetAllSensorsCsv([FromQuery(Name = "sort-by")] SensorsSortTypes sortType, string type="")
    {
        HttpContext.Response.Headers.Add("Content-Disposition", "attachment; filename=values.csv");

        String csvContent = "Type,Name,Value,Unit,Time\n";
        _sensorsService.GetAllAsync(sortType, type).Result.ForEach(sensorData=>
        {
            csvContent += $"{sensorData.Topic},{sensorData.Name},{sensorData.Value},{sensorData.UnitOfMeasurement},{sensorData.Time}\n";
        });
        return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", "values.csv");
    }
}