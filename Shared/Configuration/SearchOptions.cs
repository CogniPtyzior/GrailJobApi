namespace GrailJobApi.Shared.Configuration;

public sealed class SearchOptions
{
    public const string SectionName = "Search";

    public int MaxCriteriaWordCount { get; init; } = 20;
    public int MaxCriteriaLength { get; init; } = 500;
}
