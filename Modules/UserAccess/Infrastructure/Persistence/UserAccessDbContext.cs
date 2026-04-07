using GrailJobApi.Shared.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GrailJobApi.Modules.UserAccess.Infrastructure.Persistence;

public sealed class UserAccessDbContext(DbContextOptions<UserAccessDbContext> options)
    : IdentityUserContext<User, Guid>(options)
{
    public DbSet<SiteAccessRequest> SiteAccessRequests => Set<SiteAccessRequest>();

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

        builder.Entity<SiteAccessRequest>(entity =>
        {
            entity.ToTable("SiteAccessRequests");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.LastName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.CompanyName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.ContactEmail).HasMaxLength(320).IsRequired();
            entity.Property(x => x.JobOffer).HasMaxLength(8000).IsRequired();
            entity.Property(x => x.CreatedAtUtc).IsRequired();
            entity.Property(x => x.NotificationAttemptCount).IsRequired();

            entity.Property(x => x.NotificationStatus)
                .HasConversion<string>()
                .HasMaxLength(32)
                .IsRequired();

            entity.Property(x => x.NotificationLastError).HasMaxLength(4000);

            entity.HasIndex(x => x.CreatedAtUtc)
                .HasDatabaseName("IX_SiteAccessRequests_CreatedAtUtc");

            entity.HasIndex(x => x.ContactEmail)
                .HasDatabaseName("IX_SiteAccessRequests_ContactEmail");
        });

        builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");
    }
}