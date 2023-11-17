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
    public async Task<List<SensorValue>> GetAllSensors(
        [FromQuery(Name = "sort-by")] SensorsSortTypes sortType,
        string type= "",
        string name="",
        string dateFrom ="",
        string dateTo="")
    {
        return await _sensorsService.GetAllAsync(sortType, type, name, dateFrom, dateTo);
    }
    
    [HttpGet]
    [Route("sort-types")]
    public Dictionary<SensorsSortTypes, String> GetAllSortTypes()
    {
        return _sensorsService.GetAllSortTypes();
    }
}