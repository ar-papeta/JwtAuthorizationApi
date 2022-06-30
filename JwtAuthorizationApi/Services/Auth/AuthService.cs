using AutoMapper;
using DAL.Entities;
using DAL.Extensions;
using DAL.Repositories;
using JwtAuthorizationApi.Services.Auth.Authentication;
using JwtAuthorizationApi.Services.Extentions;
using JwtAuthorizationApi.ViewModels;
using Microsoft.Extensions.Options;
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
        private readonly IMongoRepository<User> _database;
        private readonly IMapper _mapper;
        public AuthService(
            ITokenFactory tokenFactory, 
            IConfiguration configuration,
            IMongoRepository<User> database,
            IMapper mapper,
            IOptions<MongoDbConfig> dbOptions)
        {
            _tokenFactory = tokenFactory;
            _configuration = configuration;
            _database = database;
            _mapper = mapper;
            _database.UseCollection(dbOptions.Value.UsersCollectionName);
        }

        public TokenModel CreateNewTokenModel(string userId, string userRole)
        {
            return new()
            {
                AccessToken = _tokenFactory.CreateJwtAccessToken(userId, userRole),
                RefreshToken = _tokenFactory.CreateJwtRefreshToken()
            };
        }

        public AuthenticateResponce RefreshTokens(TokenModel tokenModel)
        {
            if (tokenModel is null)
            {
                throw new ArgumentNullException(nameof(tokenModel));
            }

            _ = GetPrincipalFromToken(tokenModel.RefreshToken, _configuration.GetJwtRefreshKey(), true) 
                ?? throw new SecurityTokenException("Incoming refresh token is not correct (principals is null)");

            var principal = GetPrincipalFromToken(tokenModel.AccessToken, _configuration.GetJwtAccessKey(), false)
                ?? throw new SecurityTokenException("Incoming access token is not correct (principals is null)");

            var userId = principal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var user = _database.FilterBy(x => x.Id == userId).FirstOrDefault() 
                ?? throw new SecurityTokenException("The user with such id from token no longer exists");

            var tokens = CreateNewTokenModel(user.Id, user.Role);

            return new AuthenticateResponce
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                UserViewModel = _mapper.Map<UserViewModel>(user)
            };

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
