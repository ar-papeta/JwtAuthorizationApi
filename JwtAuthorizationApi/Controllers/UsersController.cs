using AutoMapper;
using BLL.Models;
using BLL.Services.Interfaces;
using JwtAuthorizationApi.Services.Auth;
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
        private readonly IAuthService _authService;
        private readonly IUsersService _service;
        private readonly IMapper _mapper;

        public UsersController(
            IUsersService service, 
            IAuthService authService,
            IMapper mapper)
        {
            _service = service;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("~/api/login")]
        public IActionResult ValidateUser([FromBody] AuthenticationRequest userAuthData)
        {
            var user = _service.ValidateUser(_mapper.Map<UserDto>(userAuthData));

            var tokens = _authService.CreateNewTokenModel(user.Id.ToString(), user.Role.ToString());

            Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions() { HttpOnly = true });
       
            return Ok(new AuthenticateResponce() 
            { 
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                UserViewModel = _mapper.Map<UserViewModel>(user)
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

        // GET /Logout
        [HttpGet()]
        [Route("~/api/logout")]
        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("RefreshToken"))
            {
                Response.Cookies.Delete("RefreshToken", new CookieOptions() { Expires = DateTime.Now.AddDays(-1d) });
            }
            return Ok();
        }

        // POST /Users
        [HttpPost]
        [Route("~/api/registration")]
        public IActionResult Post([FromBody] UserDto userDto)
        {
            var user = _service.CreateUser(userDto);
            var tokens = _authService.CreateNewTokenModel(user.Id.ToString(), user.Role.ToString());

            Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions() { HttpOnly = true });
            return Ok(new AuthenticateResponce()
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                UserViewModel = _mapper.Map<UserViewModel>(userDto)
            });
        }

        // POST /Users
        [HttpPost]
        [Route("~/api/auth/refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            TokenModel model = new()
            {
                AccessToken = request.AccessToken
            };

            if (Request.Cookies.ContainsKey("RefreshToken"))
            {
                model.RefreshToken = Request.Cookies["RefreshToken"]!;
            }

            var tokens = _authService.RefreshTokens(model);

            Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions() { HttpOnly = true });
            return Ok(tokens);
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
