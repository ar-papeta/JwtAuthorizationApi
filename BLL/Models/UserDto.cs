using System.Text.Json.Serialization;

namespace BLL.Models;

public class UserDto
{
    [JsonIgnore]
    public string? Id { get; set; }

    [JsonPropertyName("email")]
    public string EMail { get; set; } = null!;

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("password")]  
    public string Password { get; set; } = null!;

    [JsonPropertyName("role")]
    public string Role { get; set; } = null!;
} 

