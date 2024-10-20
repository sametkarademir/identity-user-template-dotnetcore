using Application.Dtos.Roles;
using AutoMapper;
using Domain.Models;

namespace Application.Profiles;

public class RoleMappingProfile : Profile
{
    public RoleMappingProfile()
    {
        CreateMap<AppRole, AppRoleResponseDto>().ReverseMap();
    }
}