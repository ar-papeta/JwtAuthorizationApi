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
    [Authorize]
    public IActionResult Post(Sensor newSensor)
    {
        _sensorService.CreateSensor(newSensor);
        return Ok();
    }

    [HttpDelete("{sensorId}")]
    [Authorize("sensor:read")]
    public IActionResult Delete([FromRoute] string sensorId)
    {
        return Ok(_sensorService.DeleteSensor(sensorId));
    }

    [HttpPatch("{sensorId}")]
    [Authorize("sensor:read")]
    public IActionResult Patch([FromBody] Sensor patchSensor, [FromRoute] string sensorId)
    {
        if (sensorId != patchSensor.Id)
            return BadRequest("Change sensor id is not allowed");
        return Ok(_sensorService.UpdateSensor(patchSensor));
    }

    [Authorize("sensor:read")]
    [HttpGet("{userId}")]
    public IActionResult GetUserSensors([FromRoute] string userId)
    {
        return Ok(_sensorService.GetUserSensors(userId));
    }

    [Authorize("sensor:read")]
    [HttpGet("~/api/sensor/{sensorId}")]
    public IActionResult GetSensor([FromRoute] string sensorId)
    {
        return Ok(_sensorService.GetSensorById(sensorId));
    }
}
