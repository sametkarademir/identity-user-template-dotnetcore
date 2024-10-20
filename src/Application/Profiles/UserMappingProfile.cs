using Application.Dtos.Users;
using AutoMapper;
using Core.EntityFramework.Models.Paging;
using Domain.Models;
using Domain.Shared.Dtos;

namespace Application.Profiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<AppUser, AppUserResponseDto>().ReverseMap();
        CreateMap<AppUser, CreateAppUserRequestDto>().ReverseMap();
        CreateMap<AppUser, UpdateAppUserRequestDto>().ReverseMap();
        CreateMap<Paginate<AppUser>, GetPagedListResponseDto<AppUserResponseDto>>().ReverseMap();
    }
}