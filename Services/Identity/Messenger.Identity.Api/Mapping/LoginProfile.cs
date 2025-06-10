using AutoMapper;
using Messenger.Identity.Api.DTOs;
using Messenger.Identity.Core.Models;

namespace Messenger.Identity.Api.Mapping;

public sealed class LoginProfile : Profile
{
    public LoginProfile()
    {
        CreateMap<LoginResponse, LoginResponseDto>();
    }
}
