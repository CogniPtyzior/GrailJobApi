using GrailJobApi.Modules.UserAccess.Application;

namespace GrailJobApi.Modules.UserAccess.Infrastructure.Persistence;

public sealed class SiteAccessRequestRepository(UserAccessDbContext dbContext) : ISiteAccessRequestRepository
{
    public Task AddAsync(SiteAccessRequest request, CancellationToken cancellationToken = default)
        => dbContext.SiteAccessRequests.AddAsync(request, cancellationToken).AsTask();

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}