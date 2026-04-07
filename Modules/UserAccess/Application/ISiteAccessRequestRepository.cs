namespace GrailJobApi.Modules.UserAccess.Application;

public interface ISiteAccessRequestRepository
{
    Task AddAsync(SiteAccessRequest request, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}