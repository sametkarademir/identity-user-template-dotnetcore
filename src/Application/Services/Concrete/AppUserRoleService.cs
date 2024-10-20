using Application.Dtos.Roles;
using Application.Dtos.UserRoles;
using Application.Dtos.Users;
using Application.Services.Interface;
using AutoMapper;
using Core.Exceptions.Enums;
using Core.Exceptions.Types;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Persistence.Repositories.Interface;

namespace Application.Services.Concrete;

public class AppUserRoleService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IMapper mapper, IUow uow) : IAppUserRoleService
{
    public async Task<AppUserRoleResponseDto> AssignRoleToUserAsync(AssignRoleToUserRequestDto request)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        
        if (user == null)
            throw new AppNotFoundEntityException("User not found",nameof(AppUser), request.UserId);
        
        var role = await roleManager.FindByIdAsync(request.RoleId);
        
        if (role == null)
            throw new AppNotFoundEntityException("Role not found",nameof(AppRole), request.RoleId);
        
        var result = await userManager.AddToRoleAsync(user, role.Name ?? string.Empty);
        
        if (!result.Succeeded)
            throw new AppBusinessException(result.Errors.Select(item => item.Description).ToList().ToString(), 400, AppLogLevel.Error);

        return new AppUserRoleResponseDto
        {
            UserId = user.Id,
            RoleId = role.Id
        };
    }

    public async Task DeleteRoleFromUserAsync(DeleteRoleFromUserRequestDto request)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        
        if (user == null)
            throw new AppNotFoundEntityException("User not found",nameof(AppUser), request.UserId);
        
        var role = await roleManager.FindByIdAsync(request.RoleId);
        
        if (role == null)
            throw new AppNotFoundEntityException("Role not found",nameof(AppRole), request.RoleId);
        
        var result = await userManager.RemoveFromRoleAsync(user, role.Name ?? string.Empty);
        
        if (!result.Succeeded)
            throw new AppBusinessException(result.Errors.Select(item => item.Description).ToList().ToString(), 400, AppLogLevel.Error);
    }

    public async Task<List<AppRoleResponseDto>> GetRolesByUserIdAsync(string userId)
    {
        var result = await uow.AppUserRepository.GetListByUserIdAsync(userId);
        
        return mapper.Map<List<AppRoleResponseDto>>(result.Select(x => x.AppRole).ToList());
    }

    public async Task<List<AppUserResponseDto>> GetUsersByRoleIdAsync(string roleId)
    {
        var result = await uow.AppUserRepository.GetListByRoleIdAsync(roleId);
        
        return mapper.Map<List<AppUserResponseDto>>(result.Select(x => x.AppUser).ToList());
    }
}