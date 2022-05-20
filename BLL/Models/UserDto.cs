using System.Text.Json.Serialization;

namespace BLL.Models;

public class UserDto
{
    [JsonIgnore]
    public Guid Id { get; set; }

    [JsonPropertyName("email")]
    public string EMail { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("password")]
    public string Password { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }
}

