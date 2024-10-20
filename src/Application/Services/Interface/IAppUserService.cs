using Application.Dtos.Users;
using Domain.Shared.Dtos;

namespace Application.Services.Interface;

public interface IAppUserService
{
    Task<AppUserResponseDto> GetByIdAsync(string id);
    Task<GetPagedListResponseDto<AppUserResponseDto>> GetPagedAndFilteredAsync(GetPagedAndFilteredRequestDto request);
    Task<AppUserResponseDto> CreateAsync(CreateAppUserRequestDto request);
    Task<AppUserResponseDto> UpdateAsync(string id, UpdateAppUserRequestDto request);
    Task<AppUserResponseDto> LockAsync(string id, bool isPermanent);
    Task<AppUserResponseDto> UnlockAsync(string id);
    Task<AppUserResponseDto> ChangePasswordAsync(string id, ChangePasswordAppUserRequestDto request);
    Task DeleteAsync(string id);
}