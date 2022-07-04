using AutoMapper;
using BLL.Services;
using DAL.Entities;
using JwtAuthorizationApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthorizationApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SensorsController : ControllerBase
{
    private readonly SensorsService _sensorService;
    private readonly IMapper _mapper;

    public SensorsController(SensorsService sensorsService, IMapper mapper)
    {
        _sensorService = sensorsService;
        _mapper = mapper;
    }

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
    public IActionResult Patch([FromBody] EditSensorRequestModel model, [FromRoute] string sensorId)
    {
        var patchSensor = _mapper.Map<Sensor>(model);
        patchSensor.Id = sensorId;

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
