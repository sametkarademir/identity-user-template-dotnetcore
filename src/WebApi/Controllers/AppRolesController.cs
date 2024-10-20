using Application.Dtos.Roles;
using Application.Services.Interface;
using Domain.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/roles")]
public class AppRolesController(IAppRoleService appRoleService) : ControllerBase
{
    [ProducesResponseType<AppRoleResponseDto>(200)]
    [HttpGet("{id}")]
    [Authorize(Roles = $"{RoleConsts.Admin}")]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        return Ok(await appRoleService.GetAsync(id));
    }
    
    [ProducesResponseType<List<AppRoleResponseDto>>(200)]
    [HttpGet]
    [Authorize(Roles = $"{RoleConsts.Admin}")]
    public async Task<IActionResult> GetAllAsync()
    {
        return Ok(await appRoleService.GetAllAsync());
    }
}