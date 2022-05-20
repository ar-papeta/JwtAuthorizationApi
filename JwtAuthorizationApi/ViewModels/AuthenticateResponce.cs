using System.Text.Json.Serialization;

namespace JwtAuthorizationApi.ViewModels;

public class AuthenticateResponce
{
    [JsonPropertyName("token")]
    public string Token { get; set; }

    [JsonPropertyName("user_name")]
    public string UserName { get; set; }

    [JsonPropertyName("user_role")]
    public string UserRole { get; set; }
}
