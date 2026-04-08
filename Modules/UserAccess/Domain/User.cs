using Microsoft.AspNetCore.Identity;

namespace GrailJobApi.Modules.UserAccess.Domain;

public sealed class User : IdentityUser<Guid>
{
    public const string MissingValue = "Non renseigné";

    public string FirstName { get; private set; } = MissingValue;
    public string LastName { get; private set; } = MissingValue;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? LastLoginAtUtc { get; private set; }

    private User()
    {
    }

    public static User Create(string email, string? firstName = null, string? lastName = null)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            SecurityStamp = Guid.NewGuid().ToString("N"),
            FirstName = NormalizeIdentityValue(firstName),
            LastName = NormalizeIdentityValue(lastName)
        };
    }

    public void UpdateIdentity(string? firstName, string? lastName, bool onlyWhenProvided = false)
    {
        if (!onlyWhenProvided || !string.IsNullOrWhiteSpace(firstName))
        {
            FirstName = NormalizeIdentityValue(firstName);
        }

        if (!onlyWhenProvided || !string.IsNullOrWhiteSpace(lastName))
        {
            LastName = NormalizeIdentityValue(lastName);
        }
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public string GetDisplayName()
    {
        var firstName = NormalizeDisplayValue(FirstName);
        var lastName = NormalizeDisplayValue(LastName);
        var fullName = string.Join(" ", new[] { firstName, lastName }.Where(value => !string.IsNullOrWhiteSpace(value)));

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            return fullName;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            return string.Empty;
        }

        var separatorIndex = Email.IndexOf('@');
        return separatorIndex <= 0 ? Email : Email[..separatorIndex];
    }

    public void MarkLogin(DateTime nowUtc)
    {
        LastLoginAtUtc = nowUtc;
    }

    private static string NormalizeIdentityValue(string? value)
    {
        var trimmed = value?.Trim();
        return string.IsNullOrWhiteSpace(trimmed) ? MissingValue : trimmed;
    }

    private static string NormalizeDisplayValue(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        return string.Equals(value, MissingValue, StringComparison.OrdinalIgnoreCase)
            ? string.Empty
            : value.Trim();
    }
}
