using Application.Dtos.Roles;
using Application.Dtos.UserRoles;
using Application.Dtos.Users;
using Application.Services.Interface;
using Domain.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/user-roles")]
public class AppUserRoleController(IAppUserRoleService appUserRoleService) : ControllerBase
{
    [ProducesResponseType<AppUserRoleResponseDto>(200)]
    [HttpPost]
    [Authorize(Roles = $"{RoleConsts.Admin}")]
    public async Task<IActionResult> AssignRoleToUserAsync(AssignRoleToUserRequestDto request)
    {
        var result = await appUserRoleService.AssignRoleToUserAsync(request);
        return Ok(result);
    }
    
    [ProducesResponseType<AppUserRoleResponseDto>(200)]
    [HttpDelete]
    [Authorize(Roles = $"{RoleConsts.Admin}")]
    public async Task<IActionResult> DeleteRoleFromUserAsync(DeleteRoleFromUserRequestDto request)
    {
        await appUserRoleService.DeleteRoleFromUserAsync(request);
        return Ok();
    }
    
    [ProducesResponseType<List<AppRoleResponseDto>>(200)]
    [HttpGet("roles/user/{id}")]
    [Authorize(Roles = $"{RoleConsts.Admin}")]
    public async Task<IActionResult> GetRolesByUserIdAsync(string id)
    {
        return Ok(await appUserRoleService.GetRolesByUserIdAsync(id));
    }
    
    [ProducesResponseType<List<AppUserResponseDto>>(200)]
    [HttpGet("users/role/{id}")]
    [Authorize(Roles = $"{RoleConsts.Admin}")]
    public async Task<IActionResult> GetUsersByRoleIdAsync(string id)
    {
        return Ok(await appUserRoleService.GetUsersByRoleIdAsync(id));
    }
}