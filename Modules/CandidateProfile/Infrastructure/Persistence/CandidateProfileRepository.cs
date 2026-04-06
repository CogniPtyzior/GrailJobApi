using GrailJobApi.Modules.CandidateProfile.Application;
using Microsoft.EntityFrameworkCore;
using CandidateProfileEntity = GrailJobApi.Modules.CandidateProfile.Domain.CandidateProfile;

namespace GrailJobApi.Modules.CandidateProfile.Infrastructure.Persistence;

public sealed class CandidateProfileRepository(CandidateProfileDbContext dbContext) : ICandidateProfileRepository
{
    public Task<CandidateProfileEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => dbContext.CandidateProfiles.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public Task AddAsync(CandidateProfileEntity profile, CancellationToken cancellationToken = default)
        => dbContext.CandidateProfiles.AddAsync(profile, cancellationToken).AsTask();

    public Task DeleteByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => dbContext.CandidateProfiles
            .Where(x => x.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
