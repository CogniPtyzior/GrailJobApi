using GrailJobApi.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GrailJobApi.Modules.JobSearch.Infrastructure.Persistence;

public sealed class JobSearchDbContext(DbContextOptions<JobSearchDbContext> options) : DbContext(options)
{
    public DbSet<SearchSession> SearchSessions => Set<SearchSession>();
    public DbSet<SearchCriterion> SearchCriteria => Set<SearchCriterion>();
    public DbSet<SearchResult> SearchResults => Set<SearchResult>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DbSchemas.JobSearch);

        modelBuilder.Entity<SearchSession>(entity =>
        {
            entity.ToTable("SearchSessions");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.UserId).IsUnique();
            entity.Property(x => x.Status).HasConversion<int>();
            entity.HasMany(x => x.Criteria).WithOne().HasForeignKey(x => x.SearchSessionId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(x => x.Results).WithOne().HasForeignKey(x => x.SearchSessionId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SearchCriterion>(entity =>
        {
            entity.ToTable("SearchCriteria");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Text).HasMaxLength(500);
            entity.HasIndex(x => new { x.SearchSessionId, x.Order });
            entity.HasIndex(x => new { x.SearchSessionId, x.Text }).IsUnique();
        });

        modelBuilder.Entity<SearchResult>(entity =>
        {
            entity.ToTable("SearchResults");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyName).HasMaxLength(200);
            entity.Property(x => x.JobTitle).HasMaxLength(200);
            entity.Property(x => x.OfferUrl).HasMaxLength(500);
            entity.Property(x => x.Location).HasMaxLength(200);
            entity.Property(x => x.Salary).HasMaxLength(200);
            entity.Property(x => x.TechStack).HasMaxLength(500);
            entity.Property(x => x.WorkMode).HasConversion<int>();
            entity.HasIndex(x => new { x.SearchSessionId, x.UpdatedAtUtc });
        });
    }
}
