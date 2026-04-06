namespace GrailJobApi.Modules.CompanyWorkspace.Presentation.Requests;

/// <summary>Company status update payload.</summary>
public sealed class UpdateCompanyStatusRequest
{
    /// <summary>Target status. Allowed values: Saved, Excluded.</summary>
    [Required]
    public string Status { get; init; } = string.Empty;
}
