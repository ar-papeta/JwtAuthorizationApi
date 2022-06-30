using System.Text.Json.Serialization;

namespace JwtAuthorizationApi.ViewModels
{
    public class UserViewModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("email")]
        public string EMail { get; set; } = null!;

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("role")]
        public string Role { get; set; } = null!;
    }
}
