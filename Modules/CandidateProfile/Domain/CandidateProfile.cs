namespace GrailJobApi.Modules.CandidateProfile.Domain;

public sealed class CandidateProfile
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string OriginalFileName { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long SizeInBytes { get; private set; }
    public string ExtractedText { get; private set; } = string.Empty;
    public CandidateProfileSourceType SourceType { get; private set; }
    public AiProfileInsight AiProfileInsight { get; private set; } = new(string.Empty, string.Empty);
    public DateTime ImportedAtUtc { get; private set; }

    private CandidateProfile()
    {
    }

    public static CandidateProfile Create(
        Guid userId,
        string originalFileName,
        string contentType,
        long sizeInBytes,
        string extractedText,
        CandidateProfileSourceType sourceType,
        AiProfileInsight aiProfileInsight,
        DateTime importedAtUtc)
    {
        return new CandidateProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            OriginalFileName = originalFileName,
            ContentType = contentType,
            SizeInBytes = sizeInBytes,
            ExtractedText = extractedText,
            SourceType = sourceType,
            AiProfileInsight = aiProfileInsight,
            ImportedAtUtc = importedAtUtc
        };
    }

    public void ReplaceContent(
        string originalFileName,
        string contentType,
        long sizeInBytes,
        string extractedText,
        CandidateProfileSourceType sourceType,
        AiProfileInsight aiProfileInsight,
        DateTime importedAtUtc)
    {
        OriginalFileName = originalFileName;
        ContentType = contentType;
        SizeInBytes = sizeInBytes;
        ExtractedText = extractedText;
        SourceType = sourceType;
        AiProfileInsight = aiProfileInsight;
        ImportedAtUtc = importedAtUtc;
    }
}
