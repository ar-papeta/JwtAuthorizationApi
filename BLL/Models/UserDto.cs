using System.Text.Json.Serialization;

namespace BLL.Models;

public class UserDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("email")]
    public string EMail { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonIgnore]
    public string Password { get; set; }

    [JsonPropertyName("role")]
    public string Role { get; set; }
}

