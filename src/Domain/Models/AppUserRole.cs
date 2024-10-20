using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class AppUserRole : IdentityUserRole<string>
{
    public AppRole? AppRole { get; set; }
    public AppUser? AppUser { get; set; }
}