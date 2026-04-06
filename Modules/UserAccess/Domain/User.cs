using Microsoft.AspNetCore.Identity;

namespace GrailJobApi.Modules.UserAccess.Domain;

public sealed class User : IdentityUser<Guid>
{
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? LastLoginAtUtc { get; private set; }

    private User()
    {
    }

    public static User Create(string email)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            SecurityStamp = Guid.NewGuid().ToString("N")
        };
    }

    public string GetDisplayName()
    {
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
}
