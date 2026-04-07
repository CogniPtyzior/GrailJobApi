using GrailJobApi.Modules.CandidateProfile.Application;
using GrailJobApi.Modules.CompanyWorkspace.Application;
using GrailJobApi.Modules.JobSearch.Infrastructure.Persistence;
using GrailJobApi.Modules.JobSearch.Presentation.Requests;
using GrailJobApi.Modules.JobSearch.Presentation.Responses;
using GrailJobApi.Shared.Configuration;
using Microsoft.Extensions.Options;

namespace GrailJobApi.Modules.JobSearch.Application;

public sealed class SearchService(
    ISearchSessionRepository searchSessionRepository,
    ICandidateProfileRepository candidateProfileRepository,
    IJobOpportunityRepository jobOpportunityRepository,
    IJobSearchAiClient jobSearchAiClient,
    IOptions<SearchOptions> searchOptions)
{
    private readonly SearchOptions _options = searchOptions.Value;

    public async Task<SearchCriteriaResponse> GetCriteriaAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var session = await searchSessionRepository.GetByUserIdAsync(userId, includeCriteria: true, cancellationToken: cancellationToken);
        var criteria = session?.Criteria.OrderBy(x => x.Order).Select(x => x.Text).ToArray() ?? [];
        return new SearchCriteriaResponse(criteria);
    }

    public async Task ResetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var session = await searchSessionRepository.GetByUserIdAsync(userId, includeCriteria: true, includeResults: true, cancellationToken: cancellationToken);
        if (session is not null)
        {
            await searchSessionRepository.DeleteCriteriaBySessionIdAsync(session.Id, cancellationToken);
            await searchSessionRepository.DeleteResultsBySessionIdAsync(session.Id, cancellationToken);
            session.Reset(DateTime.UtcNow);
            await searchSessionRepository.SaveChangesAsync(cancellationToken);
        }

        await jobOpportunityRepository.DeleteByUserIdAsync(userId, cancellationToken);
        await candidateProfileRepository.DeleteByUserIdAsync(userId, cancellationToken);
    }

    public async Task<SearchCriteriaResponse> SaveCriteriaAsync(Guid userId, IReadOnlyList<string> criteria, CancellationToken cancellationToken = default)
    {
        criteria ??= Array.Empty<string>();

        var normalized = NormalizeCriteria(criteria);
        var nowUtc = DateTime.UtcNow;

        var session = await searchSessionRepository.GetOrCreateAsync(userId, cancellationToken);

        await searchSessionRepository.DeleteCriteriaBySessionIdAsync(session.Id, cancellationToken);

        searchSessionRepository.AddCriteria(
            normalized.Select((text, order) => SearchCriterion.Create(session.Id, text, order, nowUtc)));

        await searchSessionRepository.SaveChangesAsync(cancellationToken);

        return new SearchCriteriaResponse(normalized);
    }

    public async Task<SearchResponse> ExecuteSearchAsync(
     Guid userId,
     IReadOnlyList<string> criteria,
     CancellationToken cancellationToken = default)
    {
        criteria ??= Array.Empty<string>();
        var normalized = NormalizeCriteria(criteria);

        if (normalized.Count == 0)
        {
            throw new InvalidOperationException("At least one search criterion is required.");
        }

        var candidateProfile = await candidateProfileRepository.GetByUserIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("A candidate profile must be uploaded before searching.");

        var filteredCompanyNames = (await jobOpportunityRepository.ListByStatusAsync(userId, JobOpportunityStatus.Saved, cancellationToken))
            .Concat(await jobOpportunityRepository.ListByStatusAsync(userId, JobOpportunityStatus.Excluded, cancellationToken))
            .Select(x => x.CompanyName)
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        var aiResults = await jobSearchAiClient.ExecuteAsync(
            candidateProfile.AiProfileInsight,
            normalized,
            filteredCompanyNames,
            cancellationToken);

        var nowUtc = DateTime.UtcNow;

        var session = await searchSessionRepository.GetOrCreateAsync(userId, cancellationToken);

        await searchSessionRepository.DeleteCriteriaBySessionIdAsync(session.Id, cancellationToken);
        await searchSessionRepository.DeleteResultsBySessionIdAsync(session.Id, cancellationToken);

        searchSessionRepository.AddCriteria(
            normalized.Select((text, order) =>
                SearchCriterion.Create(session.Id, text, order, nowUtc)));

        var persistedResults = aiResults.Select(x => SearchResult.Create(
            session.Id,
            x.CompanyName,
            x.JobTitle,
            x.OfferUrl,
            x.OfferDescription,
            x.Location,
            x.WorkMode,
            x.Salary,
            x.TechStack,
            x.MatchExplanation,
            x.RelevanceScore,
            x.UserComment,
            nowUtc))
            .ToArray();

        searchSessionRepository.AddResults(persistedResults);

        session.MarkCompleted(nowUtc);

        await searchSessionRepository.SaveChangesAsync(cancellationToken);

        return SearchResponse.From(persistedResults, hasSearched: true);
    }

    public async Task<SearchResponse> GetCurrentSearchAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var session = await searchSessionRepository.GetByUserIdAsync(userId, includeResults: true, cancellationToken: cancellationToken);
        if (session is null)
        {
            return SearchResponse.Empty();
        }

        return SearchResponse.From(session.Results.OrderByDescending(x => x.UpdatedAtUtc).ToArray(), hasSearched: session.LastExecutedAtUtc.HasValue);
    }

    private IReadOnlyList<string> NormalizeCriteria(IReadOnlyList<string> criteria)
    {
        var normalized = criteria
            .Select(x => x?.Trim() ?? string.Empty)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => string.Join(' ', x.Split(' ', StringSplitOptions.RemoveEmptyEntries)))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        foreach (var item in normalized)
        {
            if (item.Length > _options.MaxCriteriaLength)
            {
                throw new InvalidOperationException($"The criterion '{item}' exceeds the maximum allowed length.");
            }

            if (item.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length > _options.MaxCriteriaWordCount)
            {
                throw new InvalidOperationException($"The criterion '{item}' exceeds the maximum allowed number of words.");
            }
        }

        return normalized;
    }
}