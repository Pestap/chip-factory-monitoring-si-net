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
    public async Task<List<SensorValue>> GetAllSensorsValues()
    {
        return await _sensorsService.GetAllAsync();
    }
}