namespace GrailJobApi.Shared.Configuration;

public sealed class SeedOptions
{
    public const string SectionName = "Seed";
    private const string DefaultPasswordFile = "Credentials/password.secret";

    public bool EnableDevelopmentSeed { get; init; } = true;
    public bool EnableDemoData { get; init; } = true;
    public string AdminEmail { get; init; } = "admin@grailjob.local";
    public string TestEmail { get; init; } = "test@grailjob.local";
    public string PasswordFile { get; init; } = DefaultPasswordFile;

    public string ResolveAdminPassword() => ResolvePassword(PasswordFile, AdminEmail);
    public string ResolveTestPassword() => ResolvePassword(PasswordFile, TestEmail);

    private static string ResolvePassword(string? passwordFile, string email)
    {
        if (string.IsNullOrWhiteSpace(passwordFile))
        {
            return string.Empty;
        }

        var path = Path.IsPathRooted(passwordFile)
            ? passwordFile
            : Path.Combine(Directory.GetCurrentDirectory(), passwordFile);

        if (!File.Exists(path))
        {
            return string.Empty;
        }

        foreach (var line in File.ReadLines(path))
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }

            var separatorIndex = trimmedLine.IndexOf('=');
            if (separatorIndex <= 0)
            {
                continue;
            }

            var currentEmail = trimmedLine[..separatorIndex].Trim();
            if (!string.Equals(currentEmail, email, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return trimmedLine[(separatorIndex + 1)..].Trim();
        }

        return string.Empty;
    }
}