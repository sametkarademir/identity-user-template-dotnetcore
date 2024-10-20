using Core.EntityFramework.Models.Entity;

namespace Domain.Models;

public class RefreshToken : IBasicAggregateRoot<Guid>
{
    public string Token { get; set; } = null!;
    public DateTime DateCreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime DateExpiresUtc { get; set; }
    public bool IsExpired => DateTime.UtcNow >= DateExpiresUtc;
    
    public string UserId { get; set; } = null!;
    public AppUser? AppUser { get; set; }
    
    public Guid Id { get; set; }
    public bool IsDeleted { get; set; }
}