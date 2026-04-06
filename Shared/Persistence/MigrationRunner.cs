using GrailJobApi.Modules.CandidateProfile.Infrastructure.Persistence;
using GrailJobApi.Modules.CompanyWorkspace.Infrastructure.Persistence;
using GrailJobApi.Modules.JobSearch.Infrastructure.Persistence;
using GrailJobApi.Modules.UserAccess.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GrailJobApi.Shared.Persistence;

public static class MigrationRunner
{
    public static async Task ApplyMigrationsAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var provider = scope.ServiceProvider;

        await provider.GetRequiredService<UserAccessDbContext>().Database.MigrateAsync(cancellationToken);
        await provider.GetRequiredService<CandidateProfileDbContext>().Database.MigrateAsync(cancellationToken);
        await provider.GetRequiredService<JobSearchDbContext>().Database.MigrateAsync(cancellationToken);
        await provider.GetRequiredService<CompanyWorkspaceDbContext>().Database.MigrateAsync(cancellationToken);
    }
}
