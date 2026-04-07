using GrailJobApi.Modules.CandidateProfile.Domain;

namespace GrailJobApi.Modules.JobSearch.Application;
public interface IJobSearchAiClient
{
    Task<IReadOnlyList<JobSearchAiResult>> ExecuteAsync(
        AiProfileInsight candidateProfile,
        IReadOnlyList<string> criteria,
        IReadOnlyList<string> filteredCompanyNames,
        CancellationToken cancellationToken = default);
}