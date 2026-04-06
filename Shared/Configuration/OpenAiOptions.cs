namespace GrailJobApi.Shared.Configuration;

public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAi";

    public string BaseUrl { get; init; } = "https://api.openai.com/v1";
    public string? ApiKey { get; init; }
    public OpenAiModelKind Model { get; init; } = OpenAiModelKind.Gpt54;
    public double Temperature { get; init; } = 0.0;
    public int MaxOutputTokens { get; init; } = 4000;
    public bool UseMockWhenApiKeyMissing { get; init; }

    public string ResolveModelId() => Model switch
    {
        OpenAiModelKind.Gpt54 => "gpt-5.4",
        OpenAiModelKind.Gpt5Mini => "gpt-5-mini",
        OpenAiModelKind.O4Mini => "o4-mini",
        _ => "gpt-5.4"
    };
}
