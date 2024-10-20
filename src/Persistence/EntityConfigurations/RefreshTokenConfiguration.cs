using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("AppRefreshTokens");
        
        builder.HasKey(item => item.Id);
        builder.HasIndex(item => item.UserId).IsUnique();
        builder.HasQueryFilter(item => !item.IsDeleted);
        
        builder.Property(item => item.Token).HasMaxLength(1000).IsRequired();
        
        builder.HasOne<AppUser>(item => item.AppUser)
            .WithMany(item => item.RefreshTokens)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}