using Application.Dtos.Roles;

namespace Application.Services.Interface;

public interface IAppRoleService
{
    Task<AppRoleResponseDto> GetAsync(string id);
    Task<List<AppRoleResponseDto>> GetAllAsync();
}