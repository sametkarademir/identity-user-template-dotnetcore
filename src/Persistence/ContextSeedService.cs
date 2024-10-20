using System.Security.Claims;
using Domain.Models;
using Domain.Shared.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Contexts;

namespace Persistence;

public class ContextSeedService
{
    private readonly BaseDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public ContextSeedService(BaseDbContext context,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitializeContextAsync()
    {
        if (_context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Any())
        {
            await _context.Database.MigrateAsync();
        }

        if (!_roleManager.Roles.Any())
        {
            await _roleManager.CreateAsync(new AppRole { Id = Guid.NewGuid().ToString(), Name = RoleConsts.Admin });
            await _roleManager.CreateAsync(new AppRole { Id = Guid.NewGuid().ToString(), Name = RoleConsts.Guest });
        }

        if (!_userManager.Users.AnyAsync().GetAwaiter().GetResult())
        {
            var admin = new AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = UserConsts.DefaultUserName,
                Email = UserConsts.DefaultEmail,
                EmailConfirmed = true
            };

            await _userManager.CreateAsync(admin, UserConsts.DefaultPassword);
            await _userManager.AddToRolesAsync(admin, new[] { RoleConsts.Admin });
            await _userManager.AddClaimsAsync(admin, new Claim[]
            {
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.GivenName, admin.UserName)
            });
        }
    }
}