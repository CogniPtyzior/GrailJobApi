namespace GrailJobApi.Shared.Configuration;

public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAi";

    public string BaseUrl { get; init; } = "https://api.openai.com/v1";
    public string? ApiKeyFile { get; init; }
    public OpenAiModelKind Model { get; init; } = OpenAiModelKind.Gpt54;
    public double Temperature { get; init; } = 0.0;
    public int MaxOutputTokens { get; init; } = 4000;
    public bool UseMockWhenApiKeyMissing { get; init; }

    public string ResolveApiKey()
    {
        if (string.IsNullOrWhiteSpace(ApiKeyFile))
        {
            return string.Empty;
        }

        var path = Path.IsPathRooted(ApiKeyFile)
            ? ApiKeyFile
            : Path.Combine(Directory.GetCurrentDirectory(), ApiKeyFile);

        if (!File.Exists(path))
        {
            return string.Empty;
        }

        return File.ReadAllText(path).Trim();
    }

    public string ResolveModelId() => Model switch
    {
        OpenAiModelKind.Gpt54 => "gpt-5.4",
        OpenAiModelKind.Gpt5Mini => "gpt-5-mini",
        OpenAiModelKind.O4Mini => "o4-mini",
        _ => "gpt-5.4"
    };
}