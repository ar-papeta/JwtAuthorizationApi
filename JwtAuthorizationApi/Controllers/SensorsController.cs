using BLL.Services;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthorizationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly SensorsService _sensorService;

    public SensorsController(SensorsService sensorsService) => _sensorService = sensorsService;

    [HttpPost]
    public IActionResult Post(Sensor newSensor)
    {
        _sensorService.CreateSensor(newSensor);
        return Ok();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete([FromRoute] string id)
    {
        return Ok(_sensorService.DeleteSensor(id));
    }

    [Authorize("sensor:read")]
    [HttpGet("{userId}")]
    public IActionResult GetUserSensors([FromRoute] string userId)
    {
        return Ok(_sensorService.GetUserSensors(userId));
    }

    [Authorize("sensor:read")]
    [HttpGet("~/api/sensor/{sensorId}")]
    //[Route("")]
    public IActionResult GetSensor([FromRoute] string sensorId)
    {
        return Ok(_sensorService.GetSensorById(sensorId));
    }
}
