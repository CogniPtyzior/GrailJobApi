namespace GrailJobApi.Modules.UserAccess.Presentation.Requests;

public sealed class CompletePasswordSetupRequest
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public string Token { get; init; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 8)]
    [Compare(nameof(Password), ErrorMessage = "Les mots de passe saisis doivent être identiques.")]
    public string ConfirmPassword { get; init; } = string.Empty;
}
