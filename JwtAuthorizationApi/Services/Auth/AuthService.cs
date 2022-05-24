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

        public TokenModel CreateNewTokenModel(string userId, string userRole)
        {
            return new()
            {
                AccessToken = _tokenFactory.CreateJwtAccessToken(userId, userRole),
                RefreshToken = _tokenFactory.CreateJwtRefreshToken()
            };
        }

        public TokenModel RefreshTokens(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                throw new ArgumentNullException(nameof(tokenModel));
            }

            _ = GetPrincipalFromToken(tokenModel.RefreshToken, _configuration.GetJwtRefreshKey(), true) 
                ?? throw new SecurityTokenException("Incoming refresh token is not correct (principals is null)");

            var principal = GetPrincipalFromToken(tokenModel.AccessToken, _configuration.GetJwtAccessKey(), false)
                ?? throw new SecurityTokenException("Incoming access token is not correct (principals is null)");

            var userId = Guid.Parse(principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            var user = _database.Users.Get(x => x.Id == userId).First();

            return CreateNewTokenModel(user.Id.ToString(), user.Role.ToString());
        }

        private ClaimsPrincipal? GetPrincipalFromToken(string? token, string key, bool validateLifeTime)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateLifetime = validateLifeTime
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            var jwt = securityToken as JwtSecurityToken ?? throw new SecurityTokenException("Invalid access token "); ;
            if (!jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256Signature, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid access token principals");
            }
            
            return principal;
        }
    }
}
