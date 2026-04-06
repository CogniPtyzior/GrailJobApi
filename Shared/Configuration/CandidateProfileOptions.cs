namespace GrailJobApi.Shared.Configuration;

public sealed class CandidateProfileOptions
{
    public const string SectionName = "CandidateProfile";

    public long MaxFileSizeBytes { get; init; } = 2 * 1024 * 1024;
    public string[] AllowedExtensions { get; init; } = [".pdf"];
    public string[] AllowedContentTypes { get; init; } = ["application/pdf"];
}
