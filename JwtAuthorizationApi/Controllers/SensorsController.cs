using AutoMapper;
using BLL.Models;
using BLL.Services.Interfaces;
using JwtAuthorizationApi.Services.Auth;
using JwtAuthorizationApi.Services.Auth.Authentication;
using JwtAuthorizationApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthorizationApi.Controllers;

[Route("api/sensors")]
[ApiController]
public class SensorsController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUsersService _service;
    private readonly IMapper _mapper;

    public SensorsController(
        IUsersService service, 
        IAuthService authService,
        IMapper mapper)
    {
        _service = service;
        _authService = authService;
        _mapper = mapper;
    }

    
    class Sensor
    {
        public int Data { get; set; }
        public DateTime Time { get; set; }
    }
    // GET: /Sensors
    [HttpGet]
    public IActionResult Get()
    {
        Random rnd = new Random();
        Sensor[] s = new Sensor[100];
        for (int i = 0; i < 100; i++)
        {
            s[i] = new Sensor()
            {
                Data = rnd.Next(0, 100),
                Time = DateTime.Parse(DateTime.Now.AddMinutes(i).ToString("MM/dd/yyyy HH:mm:ss"))
            };
        }
        return Ok(s);
    }

    // GET /Sensors/5
    [HttpGet("{id}")]
    public string Get(Guid id)
    {
        return "Method not implemented";
    }
}
