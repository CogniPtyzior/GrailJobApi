using GrailJobApi.Modules.CandidateProfile.Presentation.Responses;
using GrailJobApi.Shared.Configuration;
using GrailJobApi.Shared.Text;
using Microsoft.Extensions.Options;
using CandidateProfileEntity = GrailJobApi.Modules.CandidateProfile.Domain.CandidateProfile;

namespace GrailJobApi.Modules.CandidateProfile.Application;

public sealed class CandidateProfileService(
    ICandidateProfileRepository repository,
    IPdfTextExtractor pdfTextExtractor,
    ICandidateProfileAiEnricher aiEnricher,
    IOptions<CandidateProfileOptions> options)
{
    private readonly CandidateProfileOptions _options = options.Value;

    public async Task<CvProfileResponse> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var profile = await repository.GetByUserIdAsync(userId, cancellationToken);
        return profile is null ? CvProfileResponse.Empty() : CvProfileResponse.From(profile);
    }

    public async Task<CvProfileResponse> UploadAsync(Guid userId, IFormFile file, CancellationToken cancellationToken = default)
    {
        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var extractedTextRaw = await pdfTextExtractor.ExtractTextAsync(memoryStream, cancellationToken);
        var extractedText = PostgresTextSanitizer.Sanitize(extractedTextRaw);

        if (string.IsNullOrWhiteSpace(extractedText))
        {
            throw new InvalidOperationException("The uploaded PDF did not contain any extractable text.");
        }

        var aiProfileInsight = await aiEnricher.EnrichAsync(extractedText, cancellationToken);
        var existing = await repository.GetByUserIdAsync(userId, cancellationToken);
        var nowUtc = DateTime.UtcNow;

        if (existing is null)
        {
            var profile = CandidateProfileEntity.Create(
                userId,
                file.FileName,
                file.ContentType,
                file.Length,
                extractedText,
                CandidateProfileSourceType.Pdf,
                aiProfileInsight,
                nowUtc);

            await repository.AddAsync(profile, cancellationToken);
            await repository.SaveChangesAsync(cancellationToken);
            return CvProfileResponse.From(profile);
        }

        existing.ReplaceContent(
            file.FileName,
            file.ContentType,
            file.Length,
            extractedText,
            CandidateProfileSourceType.Pdf,
            aiProfileInsight,
            nowUtc);

        await repository.SaveChangesAsync(cancellationToken);
        return CvProfileResponse.From(existing);
    }

    public IReadOnlyList<string> GetAllowedExtensions() => _options.AllowedExtensions;
    public IReadOnlyList<string> GetAllowedContentTypes() => _options.AllowedContentTypes;
    public long GetMaxFileSizeBytes() => _options.MaxFileSizeBytes;
}
