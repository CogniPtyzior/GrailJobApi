namespace GrailJobApi.Shared.Configuration;

public sealed class SiteAccessEmailOptions
{
    public const string SectionName = "SiteAccessEmail";

    public string RecipientEmail { get; init; } = string.Empty;
    public string FromEmail { get; init; } = string.Empty;
    public string FromName { get; init; } = "GrailJob";
    public string SmtpHost { get; init; } = string.Empty;
    public int SmtpPort { get; init; } = 587;
    public string SmtpUsername { get; init; } = string.Empty;
    public string SmtpPassword { get; init; } = string.Empty;
    public string SmtpPasswordFile { get; init; } = string.Empty;
    public bool UseStartTls { get; init; } = true;
    public string WebsiteUrl { get; init; } = string.Empty;
    public string LinkedInUrl { get; init; } = string.Empty;
    public string BrandLogoImageUrl { get; init; } = string.Empty;
    public string ProfileImageUrl { get; init; } = string.Empty;
    public string PasswordSetupUrlBase { get; init; } = string.Empty;
}