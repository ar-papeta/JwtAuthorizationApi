namespace JwtAuthorizationApi.Services.Auth.Authentication;

public interface ITokenFactory
{
    public string CreateToken(string userId, string userRole);
}
