namespace GrailJobApi.Modules.JobSearch.Application;

public interface IJobSearchAiClient
{
    Task<IReadOnlyList<JobSearchAiResult>> ExecuteAsync(
        string candidateProfileText,
        IReadOnlyList<string> criteria,
        IReadOnlyList<string> filteredCompanyNames,
        CancellationToken cancellationToken = default);
}
