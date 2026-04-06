using GrailJobApi.Modules.CandidateProfile.Infrastructure.Persistence;
using GrailJobApi.Modules.CompanyWorkspace.Infrastructure.Persistence;
using GrailJobApi.Modules.JobSearch.Infrastructure.Persistence;
using GrailJobApi.Modules.UserAccess.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GrailJobApi.Shared.Persistence;

public abstract class DesignTimeDbContextFactoryBase<TContext> : IDesignTimeDbContextFactory<TContext>
    where TContext : DbContext
{
    public TContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = DatabaseConnectionStringBuilder.Build(configuration);
        var optionsBuilder = new DbContextOptionsBuilder<TContext>();
        Configure(optionsBuilder, connectionString);
        return CreateNewInstance(optionsBuilder.Options);
    }

    protected abstract void Configure(DbContextOptionsBuilder<TContext> optionsBuilder, string connectionString);
    protected abstract TContext CreateNewInstance(DbContextOptions<TContext> options);
}

public sealed class UserAccessDbContextFactory : DesignTimeDbContextFactoryBase<UserAccessDbContext>
{
    protected override void Configure(DbContextOptionsBuilder<UserAccessDbContext> optionsBuilder, string connectionString)
        => optionsBuilder.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", DbSchemas.UserAccess));

    protected override UserAccessDbContext CreateNewInstance(DbContextOptions<UserAccessDbContext> options)
        => new(options);
}

public sealed class CandidateProfileDbContextFactory : DesignTimeDbContextFactoryBase<CandidateProfileDbContext>
{
    protected override void Configure(DbContextOptionsBuilder<CandidateProfileDbContext> optionsBuilder, string connectionString)
        => optionsBuilder.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", DbSchemas.CandidateProfile));

    protected override CandidateProfileDbContext CreateNewInstance(DbContextOptions<CandidateProfileDbContext> options)
        => new(options);
}

public sealed class JobSearchDbContextFactory : DesignTimeDbContextFactoryBase<JobSearchDbContext>
{
    protected override void Configure(DbContextOptionsBuilder<JobSearchDbContext> optionsBuilder, string connectionString)
        => optionsBuilder.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", DbSchemas.JobSearch));

    protected override JobSearchDbContext CreateNewInstance(DbContextOptions<JobSearchDbContext> options)
        => new(options);
}

public sealed class CompanyWorkspaceDbContextFactory : DesignTimeDbContextFactoryBase<CompanyWorkspaceDbContext>
{
    protected override void Configure(DbContextOptionsBuilder<CompanyWorkspaceDbContext> optionsBuilder, string connectionString)
        => optionsBuilder.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", DbSchemas.CompanyWorkspace));

    protected override CompanyWorkspaceDbContext CreateNewInstance(DbContextOptions<CompanyWorkspaceDbContext> options)
        => new(options);
}
