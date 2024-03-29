﻿using BLL.Models;
using System.Text.Json.Serialization;

namespace JwtAuthorizationApi.ViewModels;

public class AuthenticateResponce
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }

    [JsonPropertyName("user")]
    public UserViewModel UserViewModel { get; set; }



}
