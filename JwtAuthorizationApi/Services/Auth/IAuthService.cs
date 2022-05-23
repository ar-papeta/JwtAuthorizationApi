using JwtAuthorizationApi.ViewModels;

namespace JwtAuthorizationApi.Services.Auth;

public interface IAuthService
{
    public string CreateAccessToken(string userId, string userRole);
    public string RefreshToken(TokenModel tokenModel);
}
