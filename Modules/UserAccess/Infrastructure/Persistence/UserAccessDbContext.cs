using GrailJobApi.Shared.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GrailJobApi.Modules.UserAccess.Infrastructure.Persistence;

public sealed class UserAccessDbContext(DbContextOptions<UserAccessDbContext> options)
    : IdentityUserContext<User, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema(DbSchemas.UserAccess);

        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.Property(x => x.Email).HasMaxLength(320);
            entity.Property(x => x.UserName).HasMaxLength(320);
            entity.Property(x => x.NormalizedEmail).HasMaxLength(320);
            entity.Property(x => x.NormalizedUserName).HasMaxLength(320);
            entity.HasIndex(x => x.NormalizedEmail).HasDatabaseName("IX_Users_NormalizedEmail");
            entity.HasIndex(x => x.NormalizedUserName).IsUnique().HasDatabaseName("IX_Users_NormalizedUserName");
        });

        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}
