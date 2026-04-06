namespace GrailJobApi.Modules.JobSearch.Domain;

public sealed class SearchResult
{
    public Guid Id { get; private set; }
    public Guid SearchSessionId { get; private set; }
    public string CompanyName { get; private set; } = string.Empty;
    public string JobTitle { get; private set; } = string.Empty;
    public string OfferUrl { get; private set; } = string.Empty;
    public string OfferDescription { get; private set; } = string.Empty;
    public string Location { get; private set; } = string.Empty;
    public WorkMode WorkMode { get; private set; }
    public string Salary { get; private set; } = string.Empty;
    public string TechStack { get; private set; } = string.Empty;
    public string MatchExplanation { get; private set; } = string.Empty;
    public int RelevanceScore { get; private set; }
    public string UserComment { get; private set; } = string.Empty;
    public DateTime UpdatedAtUtc { get; private set; }

    private SearchResult()
    {
    }

    public static SearchResult Create(
        Guid searchSessionId,
        string companyName,
        string jobTitle,
        string offerUrl,
        string offerDescription,
        string location,
        WorkMode workMode,
        string salary,
        string techStack,
        string matchExplanation,
        int relevanceScore,
        string? userComment,
        DateTime updatedAtUtc)
    {
        return new SearchResult
        {
            Id = Guid.NewGuid(),
            SearchSessionId = searchSessionId,
            CompanyName = companyName,
            JobTitle = jobTitle,
            OfferUrl = offerUrl,
            OfferDescription = offerDescription,
            Location = location,
            WorkMode = workMode,
            Salary = salary,
            TechStack = techStack,
            MatchExplanation = matchExplanation,
            RelevanceScore = relevanceScore,
            UserComment = userComment ?? string.Empty,
            UpdatedAtUtc = updatedAtUtc
        };
    }

    public void UpdateComment(string? comment, DateTime nowUtc)
    {
        UserComment = comment?.Trim() ?? string.Empty;
        UpdatedAtUtc = nowUtc;
    }
}
