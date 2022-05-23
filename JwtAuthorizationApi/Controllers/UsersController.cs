using AutoMapper;
using BLL.Models;
using BLL.Services.Interfaces;
using JwtAuthorizationApi.Services.Auth.Authentication;
using JwtAuthorizationApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthorizationApi.Controllers
{
    [Route("api/users")]
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
        [Route("~/api/login")]
        public IActionResult ValidateUser([FromBody] AuthenticationRequest userAuthData)
        {
            var userDto = _mapper.Map<UserDto>(userAuthData);
            var user = _service.ValidateUser(userDto);

            var token = _tokenFactory.CreateJwtToken(user.Id.ToString(), user.Role.ToString());

            return Ok(new AuthenticateResponce() 
            { 
                AccessToken = token, 
                UserViewModel = _mapper.Map<UserViewModel>(userDto)
            });
        }

        // GET: /Users
        [HttpGet]
        [Authorize("user:read")]
        public IActionResult Get()
        {
            var usersDto = _service.GetUsers();

            return Ok(_mapper.Map<List<UserViewModel>>(usersDto));
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
        [Route("~/api/registration")]
        public IActionResult Post([FromBody] UserDto userDto)
        {
            var user = _service.CreateUser(userDto);
            var token = _tokenFactory.CreateJwtToken(user.Id.ToString(), user.Role.ToString());

            return Ok(new AuthenticateResponce()
            {
                AccessToken = token,
                UserViewModel = _mapper.Map<UserViewModel>(userDto)
            });
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
