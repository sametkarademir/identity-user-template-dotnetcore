using Domain.Models;

namespace Application.Services.Interface;

public interface IJwtService
{
    Task<string> CreateJwtAsync(AppUser user);
    RefreshToken CreateRefreshToken(AppUser user);
}