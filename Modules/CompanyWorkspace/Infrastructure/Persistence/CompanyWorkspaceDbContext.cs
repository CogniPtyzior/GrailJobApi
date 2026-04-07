using GrailJobApi.Shared.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GrailJobApi.Modules.CompanyWorkspace.Infrastructure.Persistence;

public sealed class CompanyWorkspaceDbContext(DbContextOptions<CompanyWorkspaceDbContext> options) : DbContext(options)
{
    public DbSet<JobOpportunity> JobOpportunities => Set<JobOpportunity>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DbSchemas.CompanyWorkspace);

        modelBuilder.Entity<JobOpportunity>(entity =>
        {
            entity.ToTable("JobOpportunities");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CompanyName).HasMaxLength(200);
            entity.Property(x => x.JobTitle).HasMaxLength(200);
            entity.Property(x => x.OfferUrl).HasMaxLength(500);
            entity.Property(x => x.Location).HasMaxLength(200);
            entity.Property(x => x.Salary).HasMaxLength(200);
            entity.Property(x => x.TechStack).HasMaxLength(500);
            entity.Property(x => x.WorkMode).HasConversion<int>();
            entity.Property(x => x.Status).HasConversion<int>();
            entity.HasIndex(x => new { x.UserId, x.Status, x.UpdatedAtUtc });
            entity.HasIndex(x => new { x.UserId, x.CompanyName }).IsUnique();
        });
    }
}