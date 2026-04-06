namespace GrailJobApi.Shared.Configuration;

public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    public bool EnableDevelopmentSeed { get; init; } = true;
    public bool EnableDemoData { get; init; } = true;
    public string AdminEmail { get; init; } = "admin@grailjob.local";
    public string AdminPassword { get; init; } = "Admin1234!";
    public string TestEmail { get; init; } = "test@grailjob.local";
    public string TestPassword { get; init; } = "Test1234!";
}
