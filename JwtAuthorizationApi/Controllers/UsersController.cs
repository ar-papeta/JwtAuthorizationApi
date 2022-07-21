using AutoMapper;
using BLL.Models;
using BLL.Services.Interfaces;
using DAL.Entities;
using JwtAuthorizationApi.Services.Auth;
using JwtAuthorizationApi.Services.Auth.Authentication;
using JwtAuthorizationApi.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace JwtAuthorizationApi.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUsersService _userService;
        private readonly IMapper _mapper;

        public UsersController(
            IUsersService userService, 
            IAuthService authService,
            IMapper mapper)
        {
            _userService = userService;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("~/api/login")]
        public IActionResult ValidateUser([FromBody] AuthenticationRequest userAuthData)
        {
            var user = _userService.ValidateUser(_mapper.Map<UserDto>(userAuthData));

            var tokens = _authService.CreateNewTokenModel(user.Id.ToString(), user.Role.ToString());

            Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions()
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = DateTime.Now.AddDays(30d),
                MaxAge = TimeSpan.FromDays(30), 
                IsEssential = true,
                
            });

            return Ok(new AuthenticateResponce() 
            { 
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                UserViewModel = _mapper.Map<UserViewModel>(user)
            });
        }

        // GET: /Users
        [HttpGet]
        [Authorize(Policy = "OnlyForAdmin")]
        public IActionResult Get()
        {
            var usersDto = _userService.GetUsers();

            return Ok(_mapper.Map<List<UserViewModel>>(usersDto));
        }

        // GET /Users/5
        [HttpGet("{userId}")]
        [Authorize("user:read")]
        public IActionResult Get(string userId)
        {
            return Ok(_mapper.Map<UserDto, UserViewModel>(_userService.GetUserById(userId)));
        }

        // PATCH /Users/5
        [HttpPatch("{userId}")]
        [Authorize("user:self")]
        public IActionResult Update([FromRoute] string userId, [FromBody] EditUserRequestModel model)
        {
            var userDto = _mapper.Map<UserDto>(model);
            userDto.Id = userId;
            return Ok(_mapper.Map<UserDto, UserViewModel>(_userService.EditUser(userDto)));
        }

        // DELETE /Users/5
        [HttpDelete("{userId}")]
        [Authorize("user:self")]
        public IActionResult Delete([FromRoute] string userId)
        {
            _userService.DeleteUser(userId);
            return NoContent();
        }

        // GET /Logout
        [HttpGet]
        [Authorize]
        [Route("~/api/logout")]
        public IActionResult Logout()
        {
            if (Request.Cookies.ContainsKey("RefreshToken"))
            {
                Response.Cookies.Delete("RefreshToken", new CookieOptions() { 
                    Expires = DateTime.Now.AddDays(-1d), 
                    SameSite = SameSiteMode.None,
                    MaxAge = TimeSpan.FromDays(30),
                    Secure = true,
                    IsEssential = true,
                });
            }
            return Ok();
        }

        // POST /Users
        [HttpPost]
        [Route("~/api/registration")]
        public IActionResult Post([FromBody] UserDto userDto)
        {
            var user = _userService.CreateUser(userDto);
            var tokens = _authService.CreateNewTokenModel(user.Id.ToString(), user.Role.ToString());

            Response.Cookies.Append("RefreshToken", tokens.RefreshToken, new CookieOptions() 
            { 
                HttpOnly = true, 
                SameSite = SameSiteMode.None, 
                Secure = true, 
                Expires = DateTime.Now.AddDays(30d),
                MaxAge = TimeSpan.FromDays(30),
                IsEssential = true,
            });
            return Ok(new AuthenticateResponce()
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                UserViewModel = _mapper.Map<UserViewModel>(user)
        });
        }


        // GET /Users
        [HttpGet]
        [Route("~/api/auth/refresh")]
        public IActionResult Refresh()
        {
            TokenModel model = new()
            {
                AccessToken = GetAccessTokenFromHeader()
            };

            if (Request.Cookies.ContainsKey("RefreshToken"))
            {
                model.RefreshToken = Request.Cookies["RefreshToken"]!;
            }
            else
            {
                throw new SecurityTokenException("Cookies does not contain a refresh token. (\"RefreshToken\")");
            }
            var response = _authService.RefreshTokens(model);
            Response.Cookies.Append("RefreshToken", model.RefreshToken, new CookieOptions() 
            { 
                HttpOnly = true, 
                SameSite = SameSiteMode.None, 
                Secure = true,
                Expires = DateTime.Now.AddDays(30d),
                MaxAge = TimeSpan.FromDays(30),
                IsEssential = true,
            });
            
            return Ok(response);
        }

        private string GetAccessTokenFromHeader()
        {
            if(!Request.Headers.TryGetValue("Authorization", out var headerValue))
            {
                throw new SecurityTokenException("Headers does not contain a authorization token.");
            }
            return headerValue.ToString()[7..];
        }
    }
}
