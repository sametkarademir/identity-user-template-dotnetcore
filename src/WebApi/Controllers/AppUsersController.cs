using Application.Dtos.Users;
using Application.Services.Interface;
using Domain.Shared.Constants;
using Domain.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/users")]
public class AppUsersController(IAppUserService appUserService) : ControllerBase
{
    
    [ProducesResponseType<AppUserResponseDto>(200)]
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetByIdAsync(string id)
    {
        return Ok(await appUserService.GetByIdAsync(id));
    }
    
    [ProducesResponseType<GetPagedListResponseDto<AppUserResponseDto>>(200)]
    [HttpPost("paged-and-filtered")]
    [Authorize(Roles = $"{RoleConsts.Admin}")]
    public async Task<IActionResult> GetPagedAndFilteredAsync(GetPagedAndFilteredRequestDto request)
    {
        return Ok(await appUserService.GetPagedAndFilteredAsync(request));
    }
    
    [ProducesResponseType<AppUserResponseDto>(200)]
    [HttpPost]
    [Authorize(Roles = $"{RoleConsts.Admin}")]
    public async Task<IActionResult> CreateAsync(CreateAppUserRequestDto request)
    {
        return Ok(await appUserService.CreateAsync(request));
    }
    
    [ProducesResponseType<AppUserResponseDto>(200)]
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAsync(string id, UpdateAppUserRequestDto request)
    {
        return Ok(await appUserService.UpdateAsync(id, request));
    }
    
    [ProducesResponseType<AppUserResponseDto>(200)]
    [HttpPut("{id}/change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePasswordAsync(string id ,ChangePasswordAppUserRequestDto request)
    {
        return Ok(await appUserService.ChangePasswordAsync(id, request));
    }
    
    [ProducesResponseType<AppUserResponseDto>(200)]
    [HttpPut("lock/{id}/{isPermanent}")]
    [Authorize(Roles = $"{RoleConsts.Admin}")]
    public async Task<IActionResult> LockAsync(string id, bool isPermanent)
    {
        return Ok(await appUserService.LockAsync(id, isPermanent));
    }
    
    [ProducesResponseType<AppUserResponseDto>(200)]
    [HttpPut("unlock/{id}")]
    [Authorize(Roles = $"{RoleConsts.Admin}")]
    public async Task<IActionResult> UnlockAsync(string id)
    {
        return Ok(await appUserService.UnlockAsync(id));
    }
    
    [ProducesResponseType(200)]
    [HttpDelete("{id}")]
    [Authorize(Roles = $"{RoleConsts.Admin}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        await appUserService.DeleteAsync(id);
        return NoContent();
    }
}