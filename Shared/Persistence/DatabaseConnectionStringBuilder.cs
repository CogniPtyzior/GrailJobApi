using GrailJobApi.Shared.Configuration;
using Npgsql;

namespace GrailJobApi.Shared.Persistence;

public static class DatabaseConnectionStringBuilder
{
    public static string Build(IConfiguration configuration)
    {
        var options = configuration.GetSection(DbOptions.SectionName).Get<DbOptions>() ?? new DbOptions();
        var password = ResolvePassword(options);

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = options.Host,
            Port = options.Port,
            Database = options.Database,
            Username = options.Username,
            Password = options.ResolvePassword(),
            IncludeErrorDetail = true
        };

        return builder.ConnectionString;
    }

    private static string ResolvePassword(DbOptions options)
    {
        if (!string.IsNullOrWhiteSpace(options.Password))
        {
            return options.Password;
        }

        if (!string.IsNullOrWhiteSpace(options.PasswordFile))
        {
            var path = Path.IsPathRooted(options.PasswordFile)
                ? options.PasswordFile
                : Path.Combine(Directory.GetCurrentDirectory(), options.PasswordFile);

            if (File.Exists(path))
            {
                return File.ReadAllText(path).Trim();
            }
        }

        return string.Empty;
    }
}
