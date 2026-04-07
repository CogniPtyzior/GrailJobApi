namespace GrailJobApi.Modules.UserAccess.Presentation.Requests;

public sealed class CreateSiteAccessRequest
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
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
    [StringLength(4000)]
    public string JobOffer { get; init; } = string.Empty;
}