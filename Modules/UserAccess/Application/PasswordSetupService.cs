using GrailJobApi.Modules.UserAccess.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GrailJobApi.Modules.UserAccess.Application;

public sealed class PasswordSetupService(
    UserManager<User> userManager,
    UserAccessDbContext dbContext)
{
    public async Task CompleteAsync(
        Guid userId,
        string token,
        string password,
        string confirmPassword,
        CancellationToken cancellationToken = default)
    {
        if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
        {
            throw new InvalidOperationException("Les mots de passe saisis doivent être identiques.");
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken)
            ?? throw new KeyNotFoundException("Utilisateur introuvable.");

        if (!user.IsActive)
        {
            throw new InvalidOperationException("Le compte utilisateur n'est pas actif.");
        }

        var result = await userManager.ResetPasswordAsync(user, token, password);
        if (result.Succeeded)
        {
            user.MarkPasswordUpdated(DateTime.UtcNow);
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        var message = result.Errors.Select(x => x.Description).FirstOrDefault();
        throw new InvalidOperationException(message ?? "Impossible de définir le mot de passe.");
    }
}
