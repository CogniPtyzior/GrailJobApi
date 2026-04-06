using GrailJobApi.Shared.Persistence;
using Microsoft.EntityFrameworkCore;
using CandidateProfileEntity = GrailJobApi.Modules.CandidateProfile.Domain.CandidateProfile;

namespace GrailJobApi.Modules.CandidateProfile.Infrastructure.Persistence;

public sealed class CandidateProfileDbContext(DbContextOptions<CandidateProfileDbContext> options) : DbContext(options)
{
    public DbSet<CandidateProfileEntity> CandidateProfiles => Set<CandidateProfileEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(DbSchemas.CandidateProfile);

        modelBuilder.Entity<CandidateProfileEntity>(entity =>
        {
            entity.ToTable("CandidateProfiles");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.UserId).IsUnique();
            entity.Property(x => x.OriginalFileName).HasMaxLength(260);
            entity.Property(x => x.ContentType).HasMaxLength(100);
            entity.Property(x => x.ExtractedText).HasColumnType("text");
            entity.Property(x => x.SourceType).HasConversion<int>();

            entity.OwnsOne(x => x.AiProfileInsight, navigation =>
            {
                navigation.Property(x => x.Title).HasColumnName("AiTitle").HasMaxLength(200);
                navigation.Property(x => x.Summary).HasColumnName("AiSummary").HasColumnType("text");
            });
        });
    }
}
