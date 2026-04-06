using GrailJobApi.Modules.CompanyWorkspace.Presentation.Responses;
using GrailJobApi.Modules.JobSearch.Application;
using GrailJobApi.Shared.Configuration;
using Microsoft.Extensions.Options;

namespace GrailJobApi.Modules.CompanyWorkspace.Application;

public sealed class CompanyWorkspaceService(
    IJobOpportunityRepository jobOpportunityRepository,
    ISearchSessionRepository searchSessionRepository,
    IOptions<CompanyWorkspaceOptions> options)
{
    private readonly CompanyWorkspaceOptions _options = options.Value;

    public async Task<CompanyListResponse> GetSavedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await jobOpportunityRepository.ListByStatusAsync(userId, JobOpportunityStatus.Saved, cancellationToken);
        return new CompanyListResponse(items.Select(CompanyResponse.FromJobOpportunity).ToArray());
    }

    public async Task<CompanyListResponse> GetExcludedAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var items = await jobOpportunityRepository.ListByStatusAsync(userId, JobOpportunityStatus.Excluded, cancellationToken);
        return new CompanyListResponse(items.Select(CompanyResponse.FromJobOpportunity).ToArray());
    }

    public async Task<CompanyResponse?> GetByIdAsync(Guid userId, Guid companyId, CancellationToken cancellationToken = default)
    {
        var tracked = await jobOpportunityRepository.GetByIdAsync(userId, companyId, cancellationToken);
        if (tracked is not null)
        {
            return CompanyResponse.FromJobOpportunity(tracked);
        }

        var currentSearchResult = await searchSessionRepository.GetResultByIdAsync(userId, companyId, cancellationToken);
        return currentSearchResult is null ? null : CompanyResponse.FromSearchResult(currentSearchResult);
    }

    public async Task<CompanyListResponse> SaveCurrentResultsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var session = await searchSessionRepository.GetByUserIdAsync(userId, includeResults: true, cancellationToken: cancellationToken);
        if (session is null || session.Results.Count == 0)
        {
            return await GetSavedAsync(userId, cancellationToken);
        }

        var nowUtc = DateTime.UtcNow;
        foreach (var result in session.Results.ToArray())
        {
            var existing = await jobOpportunityRepository.GetByUserIdAndCompanyNameAsync(userId, result.CompanyName, cancellationToken);
            if (existing is not null)
            {
                existing.ChangeStatus(JobOpportunityStatus.Saved, nowUtc);
                existing.UpdateComment(result.UserComment, nowUtc);
                continue;
            }

            await jobOpportunityRepository.AddAsync(JobOpportunity.FromSearchResult(userId, result, JobOpportunityStatus.Saved, nowUtc), cancellationToken);
        }

        session.ClearResults(nowUtc);
        await searchSessionRepository.SaveChangesAsync(cancellationToken);
        await jobOpportunityRepository.SaveChangesAsync(cancellationToken);
        return await GetSavedAsync(userId, cancellationToken);
    }

    public async Task<CompanyResponse> UpdateStatusAsync(Guid userId, Guid companyId, JobOpportunityStatus targetStatus, CancellationToken cancellationToken = default)
    {
        var tracked = await jobOpportunityRepository.GetByIdAsync(userId, companyId, cancellationToken);
        var nowUtc = DateTime.UtcNow;

        if (tracked is not null)
        {
            tracked.ChangeStatus(targetStatus, nowUtc);
            await jobOpportunityRepository.SaveChangesAsync(cancellationToken);
            return CompanyResponse.FromJobOpportunity(tracked);
        }

        var currentResult = await searchSessionRepository.GetResultByIdAsync(userId, companyId, cancellationToken)
            ?? throw new KeyNotFoundException("Company not found.");

        var existing = await jobOpportunityRepository.GetByUserIdAndCompanyNameAsync(userId, currentResult.CompanyName, cancellationToken);
        if (existing is null)
        {
            existing = JobOpportunity.FromSearchResult(userId, currentResult, targetStatus, nowUtc);
            await jobOpportunityRepository.AddAsync(existing, cancellationToken);
        }
        else
        {
            existing.ChangeStatus(targetStatus, nowUtc);
            existing.UpdateComment(currentResult.UserComment, nowUtc);
        }

        var session = await searchSessionRepository.GetByUserIdAsync(userId, includeResults: true, cancellationToken: cancellationToken)
            ?? throw new KeyNotFoundException("Search session not found.");
        var item = session.Results.First(x => x.Id == companyId);
        session.Results.Remove(item);

        await searchSessionRepository.SaveChangesAsync(cancellationToken);
        await jobOpportunityRepository.SaveChangesAsync(cancellationToken);
        return CompanyResponse.FromJobOpportunity(existing);
    }

    public async Task<CompanyResponse> UpdateCommentAsync(Guid userId, Guid companyId, string? comment, CancellationToken cancellationToken = default)
    {
        if ((comment?.Length ?? 0) > _options.MaxCommentLength)
        {
            throw new InvalidOperationException($"The comment must not exceed {_options.MaxCommentLength} characters.");
        }

        var tracked = await jobOpportunityRepository.GetByIdAsync(userId, companyId, cancellationToken);
        var nowUtc = DateTime.UtcNow;
        if (tracked is not null)
        {
            tracked.UpdateComment(comment, nowUtc);
            await jobOpportunityRepository.SaveChangesAsync(cancellationToken);
            return CompanyResponse.FromJobOpportunity(tracked);
        }

        var result = await searchSessionRepository.GetResultByIdAsync(userId, companyId, cancellationToken)
            ?? throw new KeyNotFoundException("Company not found.");

        result.UpdateComment(comment, nowUtc);
        await searchSessionRepository.SaveChangesAsync(cancellationToken);
        return CompanyResponse.FromSearchResult(result);
    }
}
