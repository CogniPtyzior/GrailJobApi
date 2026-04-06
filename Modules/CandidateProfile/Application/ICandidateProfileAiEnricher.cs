namespace GrailJobApi.Modules.CandidateProfile.Application;

public interface ICandidateProfileAiEnricher
{
    Task<AiProfileInsight> EnrichAsync(string extractedText, CancellationToken cancellationToken = default);
}
