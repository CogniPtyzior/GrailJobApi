using GrailJobApi.Modules.JobSearch.Domain;

namespace GrailJobApi.Modules.CompanyWorkspace.Domain;

public sealed class JobOpportunity
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
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
    public JobOpportunityStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    private JobOpportunity()
    {
    }

    public static JobOpportunity Create(
        Guid userId,
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
        JobOpportunityStatus status,
        DateTime nowUtc)
    {
        return new JobOpportunity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
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
            UserComment = userComment?.Trim() ?? string.Empty,
            Status = status,
            CreatedAtUtc = nowUtc,
            UpdatedAtUtc = nowUtc
        };
    }

    public static JobOpportunity FromSearchResult(Guid userId, SearchResult result, JobOpportunityStatus status, DateTime nowUtc)
        => Create(userId, result.CompanyName, result.JobTitle, result.OfferUrl, result.OfferDescription, result.Location, result.WorkMode, result.Salary, result.TechStack, result.MatchExplanation, result.RelevanceScore, result.UserComment, status, nowUtc);

    public void ChangeStatus(JobOpportunityStatus status, DateTime nowUtc)
    {
        Status = status;
        UpdatedAtUtc = nowUtc;
    }

    public void UpdateComment(string? comment, DateTime nowUtc)
    {
        UserComment = comment?.Trim() ?? string.Empty;
        UpdatedAtUtc = nowUtc;
    }
}
