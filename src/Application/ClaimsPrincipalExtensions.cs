using System.Security.Claims;

namespace Application;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    public static string? GetUserName(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.GivenName)?.Value;
    public static string? GetUserEmail(this ClaimsPrincipal user) => user.FindFirst(ClaimTypes.Email)?.Value;
    public static string? GetUserCustomProperty(this ClaimsPrincipal user, string key) => user.FindFirst(key)?.Value;
    
    public static List<string>? GetUserRole(this ClaimsPrincipal user) => user.FindAll(ClaimTypes.Role)?.Select(item => item.Value).ToList();
    public static bool IsAdmin(this ClaimsPrincipal user) => user.IsInRole("Admin");
    public static bool IsInRole(this ClaimsPrincipal user, string role) => user.GetUserRole()?.Contains(role) ?? false;
}