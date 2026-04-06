using GrailJobApi.Modules.CandidateProfile.Infrastructure.Persistence;
using GrailJobApi.Modules.CompanyWorkspace.Infrastructure.Persistence;
using GrailJobApi.Modules.JobSearch.Infrastructure.Persistence;
using GrailJobApi.Shared.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GrailJobApi.Shared.Seeding;

public sealed class DevelopmentDataSeeder(
    UserManager<User> userManager,
    IOptions<SeedOptions> seedOptions,
    CandidateProfileDbContext candidateProfileDbContext,
    JobSearchDbContext jobSearchDbContext,
    CompanyWorkspaceDbContext companyWorkspaceDbContext)
{
    private readonly SeedOptions _options = seedOptions.Value;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!_options.EnableDevelopmentSeed)
        {
            return;
        }

        var admin = await EnsureUserAsync(_options.AdminEmail, _options.AdminPassword, cancellationToken);
        var testUser = await EnsureUserAsync(_options.TestEmail, _options.TestPassword, cancellationToken);

        if (!_options.EnableDemoData)
        {
            return;
        }

        await SeedCandidateProfileAsync(testUser.Id, cancellationToken);
        await SeedSearchSessionAsync(testUser.Id, cancellationToken);
        await SeedJobOpportunitiesAsync(testUser.Id, cancellationToken);

        _ = admin;
    }

    private async Task<User> EnsureUserAsync(string email, string password, CancellationToken cancellationToken)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            return existing;
        }

        var user = User.Create(email);
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Unable to seed development user '{email}': {string.Join(", ", result.Errors.Select(x => x.Description))}");
        }

        return user;
    }

    private async Task SeedCandidateProfileAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existing = await candidateProfileDbContext.CandidateProfiles.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        if (existing is not null)
        {
            return;
        }

        var profile = CandidateProfile.Create(
            userId,
            "cv_test_user.pdf",
            "application/pdf",
            184_320,
            "Senior software engineer with strong .NET, ASP.NET Core, React, TypeScript, REST API and PostgreSQL experience. Product minded. Comfortable with hybrid or remote work.",
            CandidateProfileSourceType.Pdf,
            new AiProfileInsight("Full stack .NET / React profile", "Experienced developer with strong .NET, React, TypeScript and product-oriented delivery background."),
            DateTime.UtcNow.AddDays(-6));

        candidateProfileDbContext.CandidateProfiles.Add(profile);
        await candidateProfileDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedSearchSessionAsync(Guid userId, CancellationToken cancellationToken)
    {
        var session = await jobSearchDbContext.SearchSessions
            .Include(x => x.Criteria)
            .Include(x => x.Results)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (session is null)
        {
            session = SearchSession.Create(userId, DateTime.UtcNow.AddDays(-2));
            jobSearchDbContext.SearchSessions.Add(session);
        }

        if (session.Criteria.Count == 0)
        {
            session.ReplaceCriteria(["React TypeScript", ".NET 10 backend", "Hybrid work"], DateTime.UtcNow.AddDays(-1));
        }

        if (session.Results.Count == 0)
        {
            session.ReplaceResults([
                SearchResult.Create(session.Id, "Acme", "Senior Full Stack Developer", "https://example.com/jobs/123", "Product-oriented role with modern web stack.", "Paris", WorkMode.Hybrid, "55k-65k", ".NET 10, React, TypeScript, PostgreSQL", "Strong match for .NET and React criteria with hybrid setup.", 87, null, DateTime.UtcNow.AddHours(-8)),
                SearchResult.Create(session.Id, "Northwind", "Backend .NET Engineer", "https://example.com/jobs/456", "Modern backend role with APIs and cloud services.", "Lyon", WorkMode.Remote, "50k-60k", ".NET 10, ASP.NET Core, PostgreSQL", "Good match for backend criteria and remote flexibility.", 81, null, DateTime.UtcNow.AddHours(-7))
            ], DateTime.UtcNow.AddHours(-8));
        }

        await jobSearchDbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedJobOpportunitiesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var hasSaved = await companyWorkspaceDbContext.JobOpportunities.AnyAsync(x => x.UserId == userId && x.Status == JobOpportunityStatus.Saved, cancellationToken);
        if (!hasSaved)
        {
            companyWorkspaceDbContext.JobOpportunities.Add(JobOpportunity.Create(
                userId,
                "Contoso",
                "Backend .NET Engineer",
                "https://example.com/jobs/789",
                "Product-oriented backend role with modern .NET stack.",
                "Lyon",
                WorkMode.Remote,
                "55k-65k",
                ".NET 10, ASP.NET Core, PostgreSQL, React",
                "Strong alignment with the candidate profile thanks to modern .NET, React exposure, and remote flexibility.",
                82,
                "Very interesting mission and stack.",
                JobOpportunityStatus.Saved,
                DateTime.UtcNow.AddHours(-6)));
        }

        var hasExcluded = await companyWorkspaceDbContext.JobOpportunities.AnyAsync(x => x.UserId == userId && x.Status == JobOpportunityStatus.Excluded, cancellationToken);
        if (!hasExcluded)
        {
            companyWorkspaceDbContext.JobOpportunities.Add(JobOpportunity.Create(
                userId,
                "Fabrikam",
                "Legacy Web Developer",
                "https://example.com/jobs/999",
                "Maintenance-focused web role.",
                "Marseille",
                WorkMode.OnSite,
                "42k-48k",
                ".NET Framework, jQuery",
                "Weaker fit because the stack is older and onsite only.",
                44,
                string.Empty,
                JobOpportunityStatus.Excluded,
                DateTime.UtcNow.AddHours(-5)));
        }

        await companyWorkspaceDbContext.SaveChangesAsync(cancellationToken);
    }
}
