namespace GrailJobApi.Modules.CompanyWorkspace.Presentation.Responses;

/// <summary>Company payload used by the React frontend.</summary>
public sealed record CompanyResponse(
    string Id,
    string CompanyName,
    string JobTitle,
    string OfferUrl,
    string OfferDescription,
    string Location,
    string WorkMode,
    string Salary,
    string TechStack,
    string MatchExplanation,
    string UserComment,
    string Status,
    string UpdatedAtUtc)
{
    public static CompanyResponse FromSearchResult(SearchResult result)
        => new(
            result.Id.ToString(),
            result.CompanyName,
            result.JobTitle,
            result.OfferUrl,
            result.OfferDescription,
            result.Location,
            result.WorkMode.ToString(),
            result.Salary,
            result.TechStack,
            result.MatchExplanation,
            result.UserComment,
            "result",
            result.UpdatedAtUtc.ToString("O"));

    public static CompanyResponse FromJobOpportunity(JobOpportunity company)
        => new(
            company.Id.ToString(),
            company.CompanyName,
            company.JobTitle,
            company.OfferUrl,
            company.OfferDescription,
            company.Location,
            company.WorkMode.ToString(),
            company.Salary,
            company.TechStack,
            company.MatchExplanation,
            company.UserComment,
            company.Status == JobOpportunityStatus.Saved ? "saved" : "excluded",
            company.UpdatedAtUtc.ToString("O"));
}
