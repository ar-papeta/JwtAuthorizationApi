using DAL.Uow;
using JwtAuthorizationApi.Services.Auth.Authentication;
using JwtAuthorizationApi.Services.Extentions;
using JwtAuthorizationApi.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthorizationApi.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly ITokenFactory _tokenFactory;
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _database;
        public AuthService(
            ITokenFactory tokenFactory, 
            IConfiguration configuration,
            IUnitOfWork database)
        {
            _tokenFactory = tokenFactory;
            _configuration = configuration;
            _database = database;
        }

        public string CreateAccessToken(string userId, string userRole)
        {
            return _tokenFactory.CreateJwtToken(userId, userRole);
        }

        public string RefreshToken(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                throw new ArgumentNullException(nameof(tokenModel));
            }

            var principal = GetPrincipalFromExpiredToken(tokenModel.AccessToken);

            if (principal == null)
            {
                throw new ArgumentNullException(nameof(principal));
            }

            var user = _database.Users.GetByID(Guid.Parse(principal.Claims.First(c => c.ValueType == ClaimTypes.NameIdentifier).Value)); 



            return string.Empty;
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetJwtKey())),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid access token principals");

            return principal;

        }
    }
}
