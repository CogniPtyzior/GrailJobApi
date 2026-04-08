namespace GrailJobApi.Modules.UserAccess.Presentation.Requests;

public sealed class UpdateAdminUserRequest
{
    [StringLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [StringLength(100)]
    public string LastName { get; init; } = string.Empty;
}
