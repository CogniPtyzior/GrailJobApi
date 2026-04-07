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

                navigation.Property(x => x.TargetRoles).HasColumnName("AiTargetRoles").HasColumnType("text[]");
                navigation.Property(x => x.PreferredJobTitles).HasColumnName("AiPreferredJobTitles").HasColumnType("text[]");
                navigation.Property(x => x.CoreSkills).HasColumnName("AiCoreSkills").HasColumnType("text[]");
                navigation.Property(x => x.SecondarySkills).HasColumnName("AiSecondarySkills").HasColumnType("text[]");
                navigation.Property(x => x.MustHaveSkills).HasColumnName("AiMustHaveSkills").HasColumnType("text[]");
                navigation.Property(x => x.NiceToHaveSkills).HasColumnName("AiNiceToHaveSkills").HasColumnType("text[]");
                navigation.Property(x => x.Domains).HasColumnName("AiDomains").HasColumnType("text[]");
                navigation.Property(x => x.Industries).HasColumnName("AiIndustries").HasColumnType("text[]");
                navigation.Property(x => x.CompanyTypes).HasColumnName("AiCompanyTypes").HasColumnType("text[]");
                navigation.Property(x => x.ExperienceHighlights).HasColumnName("AiExperienceHighlights").HasColumnType("text[]");
                navigation.Property(x => x.ArchitectureFocus).HasColumnName("AiArchitectureFocus").HasColumnType("text[]");
                navigation.Property(x => x.DeliveryPractices).HasColumnName("AiDeliveryPractices").HasColumnType("text[]");
                navigation.Property(x => x.LanguagesSpoken).HasColumnName("AiLanguagesSpoken").HasColumnType("text[]");
                navigation.Property(x => x.WorkModes).HasColumnName("AiWorkModes").HasColumnType("text[]");
                navigation.Property(x => x.Locations).HasColumnName("AiLocations").HasColumnType("text[]");
                navigation.Property(x => x.MobilityArea).HasColumnName("AiMobilityArea").HasColumnType("text[]");
                navigation.Property(x => x.Certifications).HasColumnName("AiCertifications").HasColumnType("text[]");
                navigation.Property(x => x.ContractPreferences).HasColumnName("AiContractPreferences").HasColumnType("text[]");
                navigation.Property(x => x.SearchKeywords).HasColumnName("AiSearchKeywords").HasColumnType("text[]");
                navigation.Property(x => x.PersonalityTraits).HasColumnName("AiPersonalityTraits").HasColumnType("text[]");
                navigation.Property(x => x.SoftSkills).HasColumnName("AiSoftSkills").HasColumnType("text[]");
                navigation.Property(x => x.EducationDetails).HasColumnName("AiEducationDetails").HasColumnType("text[]");
                navigation.Property(x => x.Hobbies).HasColumnName("AiHobbies").HasColumnType("text[]");

                navigation.Property(x => x.Seniority).HasColumnName("AiSeniority").HasMaxLength(80);
                navigation.Property(x => x.ExperienceLevelYears).HasColumnName("AiExperienceLevelYears").HasMaxLength(80);
                navigation.Property(x => x.ManagementScope).HasColumnName("AiManagementScope").HasMaxLength(120);
            });
        });
    }
}