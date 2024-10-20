using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        // Primary key
        builder.HasKey(u => u.Id);

        // Indexes for "normalized" username and email, to allow efficient lookups
        builder.HasIndex(u => u.NormalizedUserName).HasDatabaseName("UserNameIndex").IsUnique();
        builder.HasIndex(u => u.NormalizedEmail).HasDatabaseName("EmailIndex");

        // Maps to the AspNetUsers table
        builder.ToTable("AspNetUsers");

        // A concurrency token for use with the optimistic concurrency checking
        builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

        // Limit the size of columns to use efficient database types
        builder.Property(u => u.UserName).HasMaxLength(16).IsRequired();
        builder.Property(u => u.NormalizedUserName).HasMaxLength(16).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(256).IsRequired();
        builder.Property(u => u.NormalizedEmail).HasMaxLength(256).IsRequired();

        // The relationships between User and other entity types
        // Note that these relationships are configured with no navigation properties

        // Each User can have many UserClaims
        builder.HasMany<AppUserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

        // Each User can have many UserLogins
        builder.HasMany<AppUserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

        // Each User can have many UserTokens
        builder.HasMany<AppUserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

        // Each User can have many entries in the UserRole join table
        builder.HasMany<AppUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
    }
}
public class RoleClaimMap : IEntityTypeConfiguration<AppRoleClaim>
{
    public void Configure(EntityTypeBuilder<AppRoleClaim> builder)
    {
        // Primary key
        builder.HasKey(rc => rc.Id);

        // Maps to the AspNetRoleClaims table
        builder.ToTable("AspNetRoleClaims");
    }
}
public class RoleMap : IEntityTypeConfiguration<AppRole>
{
    public void Configure(EntityTypeBuilder<AppRole> builder)
    {
        // Primary key
        builder.HasKey(r => r.Id);

        // Index for "normalized" role name to allow efficient lookups
        builder.HasIndex(r => r.NormalizedName).HasDatabaseName("RoleNameIndex").IsUnique();

        // Maps to the AspNetRoles table
        builder.ToTable("AspNetRoles");

        // A concurrency token for use with the optimistic concurrency checking
        builder.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

        // Limit the size of columns to use efficient database types
        builder.Property(u => u.Name).HasMaxLength(100).IsRequired();
        builder.Property(u => u.NormalizedName).HasMaxLength(100).IsRequired();

        // The relationships between Role and other entity types
        // Note that these relationships are configured with no navigation properties

        // Each Role can have many entries in the UserRole join table
        builder.HasMany<AppUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();

        // Each Role can have many associated RoleClaims
        builder.HasMany<AppRoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
    }
}
public class UserClaimMap : IEntityTypeConfiguration<AppUserClaim>
{
    public void Configure(EntityTypeBuilder<AppUserClaim> builder)
    {
        // Primary key
        builder.HasKey(uc => uc.Id);

        // Maps to the AspNetUserClaims table
        builder.ToTable("AspNetUserClaims");
    }
}
public class UserLoginMap : IEntityTypeConfiguration<AppUserLogin>
{
    public void Configure(EntityTypeBuilder<AppUserLogin> builder)
    {
        // Composite primary key consisting of the LoginProvider and the key to use
        // with that provider
        builder.HasKey(l => new { l.LoginProvider, l.ProviderKey });

        // Limit the size of the composite key columns due to common DB restrictions
        builder.Property(l => l.LoginProvider).HasMaxLength(128);
        builder.Property(l => l.ProviderKey).HasMaxLength(128);

        // Maps to the AspNetUserLogins table
        builder.ToTable("AspNetUserLogins");
    }
}
public class UserRoleMap : IEntityTypeConfiguration<AppUserRole>
{
    public void Configure(EntityTypeBuilder<AppUserRole> builder)
    {
        // Primary key
        builder.HasKey(r => new { r.UserId, r.RoleId });
        
        builder.HasOne<AppUser>(item => item.AppUser)
            .WithMany(item => item.AppUserRoles)
            .HasForeignKey(item => item.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne<AppRole>(item => item.AppRole)
            .WithMany(item => item.AppUserRoles)
            .HasForeignKey(item => item.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Maps to the AspNetUserRoles table
        builder.ToTable("AspNetUserRoles");
    }
}
public class UserTokenMap : IEntityTypeConfiguration<AppUserToken>
{
    public void Configure(EntityTypeBuilder<AppUserToken> builder)
    {
        // Composite primary key consisting of the UserId, LoginProvider and Name
        builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

        // Limit the size of the composite key columns due to common DB restrictions
        builder.Property(t => t.LoginProvider).HasMaxLength(256);
        builder.Property(t => t.Name).HasMaxLength(256);

        // Maps to the AspNetUserTokens table
        builder.ToTable("AspNetUserTokens");
    }
}