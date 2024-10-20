using Application.Dtos.Roles;
using Application.Dtos.UserRoles;
using Application.Dtos.Users;

namespace Application.Services.Interface;

public interface IAppUserRoleService
{
    Task<AppUserRoleResponseDto> AssignRoleToUserAsync(AssignRoleToUserRequestDto request);
    Task DeleteRoleFromUserAsync(DeleteRoleFromUserRequestDto request);
    Task<List<AppRoleResponseDto>> GetRolesByUserIdAsync(string userId);
    Task<List<AppUserResponseDto>> GetUsersByRoleIdAsync(string roleId);
}