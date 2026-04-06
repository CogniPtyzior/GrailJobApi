namespace GrailJobApi.Modules.UserAccess.Presentation.Responses;

/// <summary>Authentication session response.</summary>
public sealed record LoginResponse(UserResponse User)
{
    public static LoginResponse From(User user)
        => new(new UserResponse(user.Id.ToString(), user.Email ?? string.Empty, user.GetDisplayName()));
}

/// <summary>Authenticated user payload.</summary>
public sealed record UserResponse(string Id, string Email, string DisplayName);

public sealed record LogoutResponse(bool Success);
