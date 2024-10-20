using Application.Dtos.Users;
using Application.Services.Interface;
using AutoMapper;
using Core.EntityFramework.Models.Paging;
using Core.Exceptions.Enums;
using Core.Exceptions.Types;
using Domain.Models;
using Domain.Shared.Constants;
using Domain.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Repositories.Interface;

namespace Application.Services.Concrete;

public class AppUserService(UserManager<AppUser> userManager, IUow uow, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IAppUserService
{
    public async Task<AppUserResponseDto> GetByIdAsync(string id)
    {
        await UserRoleMustBeExist(id);
        var user = await userManager.Users.Where(x => x.UserName != "admin" && x.Id == id).FirstOrDefaultAsync();

        if (user == null)
            throw new AppNotFoundEntityException("User not found", nameof(AppUser), id);

        return mapper.Map<AppUserResponseDto>(user);
    }
    
    public async Task<GetPagedListResponseDto<AppUserResponseDto>> GetPagedAndFilteredAsync(GetPagedAndFilteredRequestDto getAllRequest)
    {
        Paginate<AppUser> result;
        if (getAllRequest.DynamicQuery is not null)
            result = await uow.AppUserRepository.GetListByDynamicAsync(
                dynamic: getAllRequest.DynamicQuery,
                index: getAllRequest.PageRequest.PageIndex,
                              size: getAllRequest.PageRequest.PageSize
                          );
        else
            result = await uow.AppUserRepository.GetListAsync(
                index: getAllRequest.PageRequest.PageIndex,
                size: getAllRequest.PageRequest.PageSize
            );
        
        return mapper.Map<GetPagedListResponseDto<AppUserResponseDto>>(result);
    }
    
    public async Task<AppUserResponseDto> CreateAsync(CreateAppUserRequestDto request)
    {
        if (await CheckUserNameExistsAsync(request.UserName))
            throw new AppBusinessException("UserName already exists", 400, AppLogLevel.Warning);
        
        if (await CheckEmailExistsAsync(request.Email))
            throw new AppBusinessException("Email already exists", 400, AppLogLevel.Warning);
            
        var user = mapper.Map<AppUser>(request);
        user.EmailConfirmed = true;
        user.Id = Guid.NewGuid().ToString();
        
        var result = await userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
            throw new AppBusinessException(result.Errors.Select(x => x.Description).ToList().ToString(), 400, AppLogLevel.Error);

        user = await userManager.FindByEmailAsync(request.Email);
        
        return mapper.Map<AppUserResponseDto>(user);
    }
    
    public async Task<AppUserResponseDto> UpdateAsync(string id, UpdateAppUserRequestDto request)
    {
        await UserRoleMustBeExist(id);
        var user = await userManager.FindByIdAsync(id);
        
        if (user == null)
            throw new AppNotFoundEntityException("User not found", nameof(AppUser), id);

        user = mapper.Map(request, user);

        var result = await userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
            throw new AppBusinessException(result.Errors.Select(x => x.Description).ToList().ToString(), 400, AppLogLevel.Error);
        
        return mapper.Map<AppUserResponseDto>(user);
    }
    
    public async Task<AppUserResponseDto> LockAsync(string id, bool isPermanent)
    {
        var user = await userManager.FindByIdAsync(id);
        
        if (user == null)
            throw new AppNotFoundEntityException("User not found", nameof(AppUser), id);
        
        await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(isPermanent ? 365 : 7));
        
        return mapper.Map<AppUserResponseDto>(user);
    }
    
    public async Task<AppUserResponseDto> UnlockAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        
        if (user == null)
            throw new AppNotFoundEntityException("User not found", nameof(AppUser), id);
        
        await userManager.SetLockoutEndDateAsync(user, null);
        
        return mapper.Map<AppUserResponseDto>(user);
    }
    
    public async Task<AppUserResponseDto> ChangePasswordAsync(string id, ChangePasswordAppUserRequestDto request)
    {
        var user = await userManager.FindByIdAsync(id);
        
        if (user == null)
            throw new AppNotFoundEntityException("User not found", nameof(AppUser), id);
        
        var result = await userManager.ChangePasswordAsync(user, request.Password, request.NewPassword);
        
        if (!result.Succeeded)
            throw new AppBusinessException(result.Errors.Select(x => x.Description).ToList().ToString(), 400, AppLogLevel.Error);
        
        return mapper.Map<AppUserResponseDto>(user);
    }
    
    public async Task DeleteAsync(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        
        if (user == null)
            throw new AppNotFoundEntityException("User not found", nameof(AppUser), id);
        
        await userManager.DeleteAsync(user);
    }
    
    
    private Task UserRoleMustBeExist(string userId)
    {
        var roles = httpContextAccessor.HttpContext?.User.GetUserRole();
        bool isManager = roles != null && roles.Any(item => item == RoleConsts.Admin);

        if (!isManager && httpContextAccessor.HttpContext?.User.GetUserId() != userId) 
            throw new AppAuthorizationException();
        
        return Task.CompletedTask;
    }

    private async Task<bool> CheckUserNameExistsAsync(string username)
    {
        var appUser = await userManager.FindByNameAsync(username);
        
        return appUser != null;
    }
    
    private async Task<bool> CheckEmailExistsAsync(string email)
    {
        var appUser =  await userManager.FindByEmailAsync(email);
        
        return appUser != null;
    }
}