namespace JwtAuthorizationApi.Services.Auth.Authentication;

public interface ITokenFactory
{
    public string CreateJwtToken(string userId, string userRole);
}
