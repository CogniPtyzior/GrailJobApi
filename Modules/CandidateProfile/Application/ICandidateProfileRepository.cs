using CandidateProfileEntity = GrailJobApi.Modules.CandidateProfile.Domain.CandidateProfile;

namespace GrailJobApi.Modules.CandidateProfile.Application;

public interface ICandidateProfileRepository
{
    Task<CandidateProfileEntity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(CandidateProfileEntity profile, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
