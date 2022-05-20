using System.Text.Json.Serialization;

namespace JwtAuthorizationApi.ViewModels;

public class UserViewModel
{
    [JsonPropertyName("email")]
    public string EMail { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }
}
