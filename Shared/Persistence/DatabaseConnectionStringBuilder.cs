using GrailJobApi.Shared.Configuration;
using Npgsql;

namespace GrailJobApi.Shared.Persistence;

public static class DatabaseConnectionStringBuilder
{
    public static string Build(IConfiguration configuration)
    {
        var options = configuration.GetSection(DbOptions.SectionName).Get<DbOptions>() ?? new DbOptions();
        var password = ResolvePassword(options);

        Console.WriteLine($"[DB DEBUG] CurrentDirectory = {Directory.GetCurrentDirectory()}");
        Console.WriteLine($"[DB DEBUG] Host = '{options.Host}'");
        Console.WriteLine($"[DB DEBUG] Port = '{options.Port}'");
        Console.WriteLine($"[DB DEBUG] Database = '{options.Database}'");
        Console.WriteLine($"[DB DEBUG] Username = '{options.Username}'");
        Console.WriteLine($"[DB DEBUG] PasswordFile = '{options.PasswordFile}'");
        Console.WriteLine($"[DB DEBUG] PasswordFile.Exists = '{(!string.IsNullOrWhiteSpace(options.PasswordFile) && File.Exists(options.PasswordFile))}'");

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = options.Host,
            Port = options.Port,
            Database = options.Database,
            Username = options.Username,
            Password = options.ResolvePassword(),
            IncludeErrorDetail = true
        };

        Console.WriteLine($"[DB DEBUG] Final Host = '{builder.Host}'");
        Console.WriteLine($"[DB DEBUG] Final Port = '{builder.Port}'");
        Console.WriteLine($"[DB DEBUG] Final Database = '{builder.Database}'");
        Console.WriteLine($"[DB DEBUG] Final Username = '{builder.Username}'");

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
