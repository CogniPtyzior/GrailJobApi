namespace GrailJobApi.Modules.UserAccess.Presentation.Requests;

public sealed class CreateSiteAccessRequest
{
    [StringLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [StringLength(100)]
    public string LastName { get; init; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string CompanyName { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string ContactEmail { get; init; } = string.Empty;

    [Required]
    [StringLength(8000)]
    public string JobOffer { get; init; } = string.Empty;
}