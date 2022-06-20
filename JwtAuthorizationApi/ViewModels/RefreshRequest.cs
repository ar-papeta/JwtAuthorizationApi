using System.Text.Json.Serialization;

namespace JwtAuthorizationApi.ViewModels
{
    public class RefreshRequest
    {
        public string AccessToken { get; set; }
    }
}
