namespace GrailJobApi.Modules.JobSearch.Presentation.Requests;

/// <summary>Search criteria payload.</summary>
public sealed class SearchCriteriaRequest
{
    /// <summary>Ordered search criteria.</summary>
    [Required]
    public List<string>? Criteria { get; init; } = [];
}
