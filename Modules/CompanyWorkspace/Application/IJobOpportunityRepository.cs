namespace GrailJobApi.Modules.CompanyWorkspace.Application;

public interface IJobOpportunityRepository
{
    Task<JobOpportunity?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
    Task<JobOpportunity?> GetByUserIdAndCompanyNameAsync(Guid userId, string companyName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JobOpportunity>> ListByStatusAsync(Guid userId, JobOpportunityStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(JobOpportunity jobOpportunity, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
