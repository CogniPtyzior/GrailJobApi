namespace GrailJobApi.Modules.UserAccess.Presentation.Requests;

public sealed class SetUserPasswordRequest
{
    [Required]
    [StringLength(200, MinimumLength = 8)]
    public string Password { get; init; } = string.Empty;
}
