namespace GrailJobApi.Shared.Configuration;

public sealed class CompanyWorkspaceOptions
{
    public const string SectionName = "CompanyWorkspace";

    public int MaxCommentLength { get; init; } = 4000;
}
