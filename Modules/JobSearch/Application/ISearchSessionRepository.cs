namespace GrailJobApi.Modules.JobSearch.Application;

public interface ISearchSessionRepository
{
    Task<SearchSession?> GetByUserIdAsync(Guid userId, bool includeCriteria = false, bool includeResults = false, CancellationToken cancellationToken = default);

    Task<SearchSession> GetOrCreateAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<SearchResult?> GetResultByIdAsync(Guid userId, Guid resultId, CancellationToken cancellationToken = default);

    Task DeleteCriteriaBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

    Task DeleteResultsBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);

    void AddCriteria(IEnumerable<SearchCriterion> criteria);

    void AddResults(IEnumerable<SearchResult> results);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
