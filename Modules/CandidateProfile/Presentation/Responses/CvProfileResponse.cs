using CandidateProfileEntity = GrailJobApi.Modules.CandidateProfile.Domain.CandidateProfile;

namespace GrailJobApi.Modules.CandidateProfile.Presentation.Responses;

/// <summary>Candidate profile summary returned to the frontend.</summary>
public sealed record CvProfileResponse(
    bool HasCv,
    DateTime? UploadedAtUtc,
    string? Title,
    string? Summary,
    string? FileName)
{
    public static CvProfileResponse Empty() => new(false, null, null, null, null);

    public static CvProfileResponse From(CandidateProfileEntity profile)
        => new(true, profile.ImportedAtUtc, profile.AiProfileInsight.Title, profile.AiProfileInsight.Summary, profile.OriginalFileName);
}
