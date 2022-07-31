using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthorizationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsDataController : ControllerBase
{
    private readonly SensorsDataService _sensorDataService;

    public SensorsDataController(SensorsDataService sensorsDataService) => _sensorDataService = sensorsDataService;

    [HttpPost("{sensorId}")]
    public IActionResult Post([FromRoute] string sensorId, [FromBody] SensorData sensorsData)
    {
        _sensorDataService.InsertSensorData(sensorsData, sensorId);
        return Ok();
    }

    [Authorize("sensor:read")]
    [HttpGet("{sensorId}/{period?}")]
    public IActionResult GetDataFromFixPeriod([FromRoute] string sensorId, [FromRoute] string? period)
    {
        return Ok(_sensorDataService.GetSensorDataFromPeriod(sensorId, period));
    }

    [Authorize("sensor:read")]
    [HttpGet("{sensorId}")]
    public IActionResult GetData(string sensorId, DateTime from, DateTime to, int dataCount)
    {
        return Ok(_sensorDataService.GetSensorDataFromPeriod(sensorId, from, to, dataCount));
    }
}
