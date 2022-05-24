using System.Text.Json.Serialization;

namespace JwtAuthorizationApi.ViewModels
{
    public class RefreshRequest
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
