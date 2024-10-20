using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class AppRole : IdentityRole<string>
{
    public ICollection<AppUserRole>? AppUserRoles { get; set; }
}