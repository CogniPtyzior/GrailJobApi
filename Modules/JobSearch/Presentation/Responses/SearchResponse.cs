using GrailJobApi.Modules.CompanyWorkspace.Presentation.Responses;

namespace GrailJobApi.Modules.JobSearch.Presentation.Responses;

public sealed record SearchResponse(bool HasSearched, IReadOnlyList<CompanyResponse> Results)
{
    public static SearchResponse Empty() => new(false, []);

    public static SearchResponse From(IEnumerable<SearchResult> results, bool hasSearched)
        => new(hasSearched, results.Select(CompanyResponse.FromSearchResult).ToArray());
}
