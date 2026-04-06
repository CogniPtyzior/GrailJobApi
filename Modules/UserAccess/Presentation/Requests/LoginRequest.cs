namespace GrailJobApi.Modules.UserAccess.Presentation.Requests;

/// <summary>Authentication request payload.</summary>
public sealed class LoginRequest
{
    /// <summary>User email.</summary>
    [Required]
    [EmailAddress]
    [StringLength(320)]
    public string Email { get; init; } = string.Empty;

    /// <summary>User password.</summary>
    [Required]
    [StringLength(200)]
    public string Password { get; init; } = string.Empty;
}
