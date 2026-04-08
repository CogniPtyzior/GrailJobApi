using GrailJobApi.Modules.UserAccess.Infrastructure.Persistence;
using GrailJobApi.Modules.UserAccess.Presentation.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GrailJobApi.Modules.UserAccess.Application;

public sealed class AdminUserAccessService(
    UserManager<User> userManager,
    UserAccessDbContext dbContext,
    IPasswordSetupEmailSender passwordSetupEmailSender,
    ILogger<AdminUserAccessService> logger)
{
    private const int MaxPageSize = 15;

    public async Task<PagedResponse<UserResponse>> GetUsersAsync(
        int page,
        int pageSize,
        string? status,
        string? firstName,
        string? lastName,
        string? email,
        CancellationToken cancellationToken = default)
    {
        var safePageSize = ClampPageSize(pageSize);
        IQueryable<User> query = dbContext.Users.AsNoTracking();

        if (string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(x => x.IsActive);
        }
        else if (string.Equals(status, "inactive", StringComparison.OrdinalIgnoreCase))
        {
            query = query.Where(x => !x.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(firstName))
        {
            var firstNameFilter = firstName.Trim().ToLowerInvariant();
            query = query.Where(x => x.FirstName.ToLower().Contains(firstNameFilter));
        }

        if (!string.IsNullOrWhiteSpace(lastName))
        {
            var lastNameFilter = lastName.Trim().ToLowerInvariant();
            query = query.Where(x => x.LastName.ToLower().Contains(lastNameFilter));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailFilter = email.Trim().ToLowerInvariant();
            query = query.Where(x => (x.Email ?? string.Empty).ToLower().Contains(emailFilter));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)safePageSize));
        var currentPage = Math.Min(Math.Max(page, 1), totalPages);

        var items = await query
            .OrderByDescending(x => x.IsActive)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Skip((currentPage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<UserResponse>(
            items.Select(UserResponse.From).ToArray(),
            currentPage,
            safePageSize,
            totalCount);
    }

    public async Task<UserResponse> UpdateUserIdentityAsync(
        Guid userId,
        string? firstName,
        string? lastName,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userId, cancellationToken);
        user.UpdateIdentity(firstName, lastName);

        var result = await userManager.UpdateAsync(user);
        EnsureSucceeded(result, "Impossible de mettre à jour le nom de l'utilisateur.");

        return UserResponse.From(user);
    }

    public async Task<UserResponse> SetUserStatusAsync(
        Guid currentUserId,
        Guid userId,
        bool isActive,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userId, cancellationToken);

        if (!isActive && user.Id == currentUserId)
        {
            throw new InvalidOperationException("Le compte actuellement connecté ne peut pas être désactivé.");
        }

        if (isActive)
        {
            user.Activate();
        }
        else
        {
            user.Deactivate();
        }

        var result = await userManager.UpdateAsync(user);
        EnsureSucceeded(result, "Impossible de modifier l'état de l'utilisateur.");

        return UserResponse.From(user);
    }

    public async Task<UserResponse> SetUserPasswordAsync(
        Guid userId,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userId, cancellationToken);

        if (await userManager.HasPasswordAsync(user))
        {
            var removePasswordResult = await userManager.RemovePasswordAsync(user);
            EnsureSucceeded(removePasswordResult, "Impossible de remplacer le mot de passe existant.");
        }

        var addPasswordResult = await userManager.AddPasswordAsync(user, password);
        EnsureSucceeded(addPasswordResult, "Impossible de définir le mot de passe.");

        user.MarkPasswordUpdated(DateTime.UtcNow);
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        var refreshedUser = await dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == userId, cancellationToken);
        return UserResponse.From(refreshedUser);
    }

    public async Task<UserResponse> SendPasswordResetLinkAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await FindUserAsync(userId, cancellationToken);
        if (!user.IsActive)
        {
            throw new InvalidOperationException("Le lien de réinitialisation ne peut être envoyé qu'à un compte actif.");
        }

        await TrySendPasswordSetupEmailAsync(user, cancellationToken);

        var refreshedUser = await dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == userId, cancellationToken);
        return UserResponse.From(refreshedUser);
    }

    public async Task<PagedResponse<AdminSiteAccessRequestResponse>> GetSiteAccessRequestsAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var safePageSize = ClampPageSize(pageSize);
        var totalCount = await dbContext.SiteAccessRequests.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)safePageSize));
        var currentPage = Math.Min(Math.Max(page, 1), totalPages);

        var items = await dbContext.SiteAccessRequests
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Skip((currentPage - 1) * safePageSize)
            .Take(safePageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<AdminSiteAccessRequestResponse>(
            items.Select(AdminSiteAccessRequestResponse.From).ToArray(),
            currentPage,
            safePageSize,
            totalCount);
    }

    public async Task<AdminSiteAccessRequestResponse> UpdateSiteAccessRequestIdentityAsync(
        Guid requestId,
        string? firstName,
        string? lastName,
        CancellationToken cancellationToken = default)
    {
        var request = await dbContext.SiteAccessRequests.FirstOrDefaultAsync(x => x.Id == requestId, cancellationToken);
        if (request is null)
        {
            throw new KeyNotFoundException("Demande d'accès introuvable.");
        }

        request.UpdateIdentity(firstName, lastName);
        await dbContext.SaveChangesAsync(cancellationToken);

        return AdminSiteAccessRequestResponse.From(request);
    }

    public async Task<UserResponse> AcceptSiteAccessRequestAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var request = await dbContext.SiteAccessRequests.FirstOrDefaultAsync(x => x.Id == requestId, cancellationToken);
        if (request is null)
        {
            throw new KeyNotFoundException("Demande d'accès introuvable.");
        }

        var normalizedEmail = NormalizeEmail(request.ContactEmail);
        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            throw new InvalidOperationException("L'email de la demande est invalide.");
        }

        var existingUser = await dbContext.Users.FirstOrDefaultAsync(
            x => x.NormalizedEmail == normalizedEmail,
            cancellationToken);

        User user;
        var shouldSendPasswordSetupEmail = false;

        if (existingUser is null)
        {
            user = User.Create(request.ContactEmail.Trim(), request.FirstName, request.LastName);

            var createResult = await userManager.CreateAsync(user);
            EnsureSucceeded(createResult, "Impossible de créer l'utilisateur.");

            shouldSendPasswordSetupEmail = true;
        }
        else
        {
            user = existingUser;
            user.UpdateIdentity(request.FirstName, request.LastName, onlyWhenProvided: true);

            if (!user.IsActive)
            {
                user.Activate();
                shouldSendPasswordSetupEmail = true;
            }
            else if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                shouldSendPasswordSetupEmail = true;
            }

            var updateResult = await userManager.UpdateAsync(user);
            EnsureSucceeded(updateResult, "Impossible de réactiver l'utilisateur.");
        }

        if (shouldSendPasswordSetupEmail)
        {
            await TrySendPasswordSetupEmailAsync(user, cancellationToken);
        }

        var requestsToDelete = await dbContext.SiteAccessRequests
            .Where(x => x.ContactEmail.ToUpper() == normalizedEmail)
            .ToListAsync(cancellationToken);

        dbContext.SiteAccessRequests.RemoveRange(requestsToDelete);
        await dbContext.SaveChangesAsync(cancellationToken);

        var refreshedUser = await dbContext.Users.AsNoTracking().FirstAsync(x => x.Id == user.Id, cancellationToken);
        return UserResponse.From(refreshedUser);
    }

    public async Task DeleteSiteAccessRequestAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var request = await dbContext.SiteAccessRequests.FirstOrDefaultAsync(x => x.Id == requestId, cancellationToken);
        if (request is null)
        {
            throw new KeyNotFoundException("Demande d'accès introuvable.");
        }

        dbContext.SiteAccessRequests.Remove(request);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<User> FindUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        return user ?? throw new KeyNotFoundException("Utilisateur introuvable.");
    }

    private static string NormalizeEmail(string email)
        => email.Trim().ToUpperInvariant();

    private static int ClampPageSize(int pageSize)
        => pageSize <= 0 ? MaxPageSize : Math.Min(pageSize, MaxPageSize);

    private async Task<bool> TrySendPasswordSetupEmailAsync(
        User user,
        CancellationToken cancellationToken)
    {
        var attemptedAtUtc = DateTime.UtcNow;

        try
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var passwordSetupLink = BuildPasswordSetupLink(user.Id, token);
            await passwordSetupEmailSender.SendAsync(user, passwordSetupLink, cancellationToken);

            user.MarkPasswordResetLinkDispatch(attemptedAtUtc, succeeded: true);
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception exception)
        {
            user.MarkPasswordResetLinkDispatch(attemptedAtUtc, succeeded: false);
            dbContext.Users.Update(user);
            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogError(
                exception,
                "Échec de l'envoi du lien de définition du mot de passe pour l'utilisateur {UserId}.",
                user.Id);

            return false;
        }
    }

    private static void EnsureSucceeded(IdentityResult result, string defaultMessage)
    {
        if (result.Succeeded)
        {
            return;
        }

        var message = result.Errors.Select(x => x.Description).FirstOrDefault();
        throw new InvalidOperationException(message ?? defaultMessage);
    }

    private string BuildPasswordSetupLink(Guid userId, string token)
    {
        var baseUrl = passwordSetupEmailSender.PasswordSetupUrlBase.Trim();
        var separator = baseUrl.Contains('?', StringComparison.Ordinal) ? "&" : "?";

        return $"{baseUrl}{separator}userId={Uri.EscapeDataString(userId.ToString())}&token={Uri.EscapeDataString(token)}";
    }
}
