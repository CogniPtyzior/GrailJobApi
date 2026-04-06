using GrailJobApi.Modules.UserAccess.Presentation.Requests;
using GrailJobApi.Modules.UserAccess.Presentation.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace GrailJobApi.Modules.UserAccess.Application;

public sealed class AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.IsActive)
        {
            return null;
        }

        var result = await signInManager.PasswordSignInAsync(user, request.Password, isPersistent: true, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            return null;
        }

        user.MarkLogin(DateTime.UtcNow);
        await userManager.UpdateAsync(user);
        return LoginResponse.From(user);
    }

    public Task LogoutAsync(HttpContext httpContext)
        => httpContext.SignOutAsync(IdentityConstants.ApplicationScheme);

    public async Task<LoginResponse?> GetCurrentAsync(ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        return user is null ? null : LoginResponse.From(user);
    }
}
