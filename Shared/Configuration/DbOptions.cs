namespace GrailJobApi.Shared.Configuration;

public sealed class DbOptions
{
    public const string SectionName = "Db";

    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 5432;
    public string Database { get; init; } = "grailjob";
    public string Username { get; init; } = "grailjob";
    public string? Password { get; init; }
    public string? PasswordFile { get; init; }
    public string ResolvePassword()
    {
        if (!string.IsNullOrWhiteSpace(PasswordFile) && File.Exists(PasswordFile))
        {
            return File.ReadAllText(PasswordFile).Trim();
        }

        return Password ?? string.Empty;
    }
}
