using GrailJobApi.Modules.JobSearch.Application;
using Microsoft.EntityFrameworkCore;

namespace GrailJobApi.Modules.JobSearch.Infrastructure.Persistence;

public sealed class SearchSessionRepository(JobSearchDbContext dbContext) : ISearchSessionRepository
{
    public async Task<SearchSession?> GetByUserIdAsync(Guid userId, bool includeCriteria = false, bool includeResults = false, CancellationToken cancellationToken = default)
    {
        IQueryable<SearchSession> query = dbContext.SearchSessions;

        if (includeCriteria)
        {
            query = query.Include(x => x.Criteria);
        }

        if (includeResults)
        {
            query = query.Include(x => x.Results);
        }

        return await query.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<SearchSession> GetOrCreateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var session = await dbContext.SearchSessions
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        if (session is not null)
        {
            return session;
        }

        session = SearchSession.Create(userId, DateTime.UtcNow);
        await dbContext.SearchSessions.AddAsync(session, cancellationToken);

        return session;
    }

    public async Task<SearchResult?> GetResultByIdAsync(Guid userId, Guid resultId, CancellationToken cancellationToken = default)
    {
        return await dbContext.SearchResults
            .Where(x => x.Id == resultId)
            .Join(dbContext.SearchSessions, r => r.SearchSessionId, s => s.Id, (result, session) => new { result, session.UserId })
            .Where(x => x.UserId == userId)
            .Select(x => x.result)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task DeleteCriteriaBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default) =>
        dbContext.SearchCriteria
            .Where(x => x.SearchSessionId == sessionId)
            .ExecuteDeleteAsync(cancellationToken);

    public Task DeleteResultsBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
    => dbContext.SearchResults
        .Where(x => x.SearchSessionId == sessionId)
        .ExecuteDeleteAsync(cancellationToken);

    public void AddCriteria(IEnumerable<SearchCriterion> criteria)
        => dbContext.SearchCriteria.AddRange(criteria);

    public void AddResults(IEnumerable<SearchResult> results)
        => dbContext.SearchResults.AddRange(results);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => dbContext.SaveChangesAsync(cancellationToken);
}
