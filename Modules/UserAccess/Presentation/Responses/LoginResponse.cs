namespace GrailJobApi.Modules.UserAccess.Presentation.Responses;

/// <summary>Authentication session response.</summary>
public sealed record LoginResponse(UserResponse User)
{
    public static LoginResponse From(User user)
        => new(UserResponse.From(user));
}

/// <summary>Authenticated user payload.</summary>
public sealed record UserResponse(
    string Id,
    string Email,
    string DisplayName,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTime CreatedAtUtc,
    DateTime? LastLoginAtUtc,
    bool HasPassword)
{
    public static UserResponse From(User user)
        => new(
            user.Id.ToString(),
            user.Email ?? string.Empty,
            user.GetDisplayName(),
            user.FirstName,
            user.LastName,
            user.IsActive,
            user.CreatedAtUtc,
            user.LastLoginAtUtc,
            !string.IsNullOrWhiteSpace(user.PasswordHash));
}

public sealed record LogoutResponse(bool Success);
