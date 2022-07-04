using System.Text.Json.Serialization;

namespace JwtAuthorizationApi.ViewModels;

public class EditUserRequestModel
{
    [JsonPropertyName("email")]
    public string? EMail { get; set; } 

    [JsonPropertyName("name")]
    public string? Name { get; set; } 
}
