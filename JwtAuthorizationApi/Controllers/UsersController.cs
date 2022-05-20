using AutoMapper;
using BLL.Models;
using BLL.Services.Interfaces;
using JwtAuthorizationApi.Services.Auth.Authentication;
using JwtAuthorizationApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthorizationApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ITokenFactory _tokenFactory;
        private readonly IUsersService _service;
        private readonly IMapper _mapper;

        public UsersController(
            IUsersService service, 
            ITokenFactory tokenFactory,
            IMapper mapper)
        {
            _service = service;
            _tokenFactory = tokenFactory;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("~/actions/login")]
        public IActionResult ValidateUser([FromBody] AuthenticationRequest userAuthData)
        {
            var userDto = _mapper.Map<UserDto>(userAuthData);
            var user = _service.ValidateUser(userDto);

            var token = _tokenFactory.CreateToken(user.Id.ToString(), user.Role.ToString());

            return Ok(new AuthenticateResponce() { Token = token, UserName = user.Name, UserRole = user.Role.ToString() });
        }

        // GET: /Users
        [HttpGet]
        [Authorize("user:read")]
        public IActionResult Get()
        {
            var usersDto = _service.GetUsers();
            var users = _mapper.Map<List<UserDto>, List<UserViewModel>>(usersDto.ToList());
            
            return Ok(users);
        }

        // GET /Users/5
        [HttpGet("{id}")]
        [Authorize("user:read")]
        public string Get(Guid id)
        {
            return "Method not implemented";
        }

        // POST /Users
        [HttpPost]
        public IActionResult Post([FromBody] UserDto userDto)
        {
            var user = _service.CreateUser(userDto);
            var token = _tokenFactory.CreateToken(user.Id.ToString(), user.Role.ToString());

            return Ok(new AuthenticateResponce() { Token = token, UserName = user.Name, UserRole = user.Role.ToString() });
        }

        // PUT /Users/5
        [HttpPut("{id}")]
        public void Put(Guid id, [FromBody] string value)
        {
        }

        // DELETE /Users/5
        [HttpDelete("{id}")]
        public void Delete(Guid id)
        {
        }
    }
}
