using static System.Collections.Specialized.BitVector32;

namespace GrailJobApi.Modules.JobSearch.Domain;

public sealed class SearchSession
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public SearchSessionStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }
    public DateTime? LastExecutedAtUtc { get; private set; }

    public ICollection<SearchCriterion> Criteria { get; private set; } = [];
    public ICollection<SearchResult> Results { get; private set; } = [];

    private SearchSession()
    {
    }

    public static SearchSession Create(Guid userId, DateTime nowUtc)
    {
        return new SearchSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = SearchSessionStatus.Idle,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc
        };
    }

    public void ReplaceCriteria(IReadOnlyList<string> criteria, DateTime nowUtc)
    {
        Criteria.Clear();
        for (var i = 0; i < criteria.Count; i++)
        {
            Criteria.Add(SearchCriterion.Create(Id, criteria[i], i, nowUtc));
        }

        UpdatedAtUtc = nowUtc;
    }

    public void ReplaceResults(IEnumerable<SearchResult> results, DateTime nowUtc)
    {
        Results.Clear();
        foreach (var result in results)
        {
            Results.Add(result);
        }

        Status = SearchSessionStatus.Completed;
        LastExecutedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }

    public void ClearResults(DateTime nowUtc)
    {
        Results.Clear();
        UpdatedAtUtc = nowUtc;
    }

    public void Touch(DateTime nowUtc)
    {
        UpdatedAtUtc = nowUtc;
    }

    public void MarkCompleted(DateTime nowUtc)
    {
        Status = SearchSessionStatus.Completed;
        LastExecutedAtUtc = nowUtc;
        UpdatedAtUtc = nowUtc;
    }
}
