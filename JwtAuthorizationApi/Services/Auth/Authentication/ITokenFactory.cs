namespace JwtAuthorizationApi.Services.Auth.Authentication;

public interface ITokenFactory
{
    public string CreateJwtAccessToken(string userId, string userRole);
    public string CreateJwtRefreshToken();
}
