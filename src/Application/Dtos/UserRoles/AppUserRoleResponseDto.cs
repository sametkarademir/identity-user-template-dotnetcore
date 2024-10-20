using Application.Dtos.Roles;
using Application.Dtos.Users;

namespace Application.Dtos.UserRoles;

public class AppUserRoleResponseDto
{
    public string UserId { get; set; } = null!;
    public AppUserResponseDto? AppUser { get; set; }

    public string RoleId { get; set; } = null!;
    public AppRoleResponseDto? AppRole { get; set; }
}