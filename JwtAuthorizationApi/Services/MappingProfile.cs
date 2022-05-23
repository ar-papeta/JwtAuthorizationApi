using AutoMapper;
using BLL.Models;
using DAL.Entities;
using JwtAuthorizationApi.ViewModels;

namespace JwtAuthorizationApi.Services;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        
        CreateMap<AuthenticationRequest, UserDto>();
        CreateMap<UserDto, AuthenticationRequest>();
    }
}
