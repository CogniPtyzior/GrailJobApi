namespace GrailJobApi.Modules.CandidateProfile.Domain;

public sealed class AiProfileInsight
{
    public string Title { get; private set; } = string.Empty;
    public string Summary { get; private set; } = string.Empty;

    private AiProfileInsight()
    {
    }

    public AiProfileInsight(string title, string summary)
    {
        Title = title;
        Summary = summary;
    }
}
