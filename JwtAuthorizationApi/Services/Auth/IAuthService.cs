using JwtAuthorizationApi.ViewModels;

namespace JwtAuthorizationApi.Services.Auth;

public interface IAuthService
{
    public TokenModel CreateNewTokenModel(string userId, string userRole);
    public AuthenticateResponce RefreshTokens(TokenModel tokenModel);
}
