using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class AppUser : IdentityUser<string>
{
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
    public ICollection<AppUserRole>? AppUserRoles { get; set; }
}