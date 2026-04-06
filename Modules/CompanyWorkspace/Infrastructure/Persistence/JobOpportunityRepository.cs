using GrailJobApi.Modules.CompanyWorkspace.Application;
using Microsoft.EntityFrameworkCore;

namespace GrailJobApi.Modules.CompanyWorkspace.Infrastructure.Persistence;

public sealed class JobOpportunityRepository(CompanyWorkspaceDbContext dbContext) : IJobOpportunityRepository
{
    public Task<JobOpportunity?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
        => dbContext.JobOpportunities.FirstOrDefaultAsync(x => x.UserId == userId && x.Id == id, cancellationToken);

    public Task<JobOpportunity?> GetByUserIdAndCompanyNameAsync(Guid userId, string companyName, CancellationToken cancellationToken = default)
        => dbContext.JobOpportunities
            .Where(x => x.UserId == userId && x.CompanyName == companyName)
            .OrderByDescending(x => x.UpdatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<JobOpportunity>> ListByStatusAsync(Guid userId, JobOpportunityStatus status, CancellationToken cancellationToken = default)
        => await dbContext.JobOpportunities
            .Where(x => x.UserId == userId && x.Status == status)
            .OrderByDescending(x => x.UpdatedAtUtc)
            .ToListAsync(cancellationToken);

    public Task AddAsync(JobOpportunity jobOpportunity, CancellationToken cancellationToken = default)
        => dbContext.JobOpportunities.AddAsync(jobOpportunity, cancellationToken).AsTask();

    public Task DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => dbContext.JobOpportunities
            .Where(x => x.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
