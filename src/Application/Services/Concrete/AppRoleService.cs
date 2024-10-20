using Application.Dtos.Roles;
using Application.Services.Interface;
using AutoMapper;
using Core.Exceptions.Types;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Concrete;

public class AppRoleService(RoleManager<AppRole> roleManager, IMapper mapper) : IAppRoleService
{
    public async Task<AppRoleResponseDto> GetAsync(string id)
    {
        var result = await roleManager.FindByIdAsync(id);
        
        if (result is null)
            throw new AppNotFoundEntityException("Role not found", nameof(AppRole), id);

        return mapper.Map<AppRoleResponseDto>(result);
    }

    public async Task<List<AppRoleResponseDto>> GetAllAsync()
    {
        var result = await roleManager.Roles.ToListAsync();
        
        return mapper.Map<List<AppRoleResponseDto>>(result);
    }
}