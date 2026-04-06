namespace GrailJobApi.Modules.JobSearch.Domain;

public sealed class SearchCriterion
{
    public Guid Id { get; private set; }
    public Guid SearchSessionId { get; private set; }
    public string Text { get; private set; } = string.Empty;
    public int Order { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private SearchCriterion()
    {
    }

    internal static SearchCriterion Create(Guid searchSessionId, string text, int order, DateTime nowUtc)
        => new()
        {
            Id = Guid.NewGuid(),
            SearchSessionId = searchSessionId,
            Text = text,
            Order = order,
            CreatedAtUtc = nowUtc
        };
}
