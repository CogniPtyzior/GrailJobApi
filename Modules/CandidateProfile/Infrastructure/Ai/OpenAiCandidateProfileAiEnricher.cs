using GrailJobApi.Modules.CandidateProfile.Application;
using GrailJobApi.Shared.Ai;

namespace GrailJobApi.Modules.CandidateProfile.Infrastructure.Ai;

public sealed class OpenAiCandidateProfileAiEnricher(OpenAiStructuredChatClient client) : ICandidateProfileAiEnricher
{
    public async Task<AiProfileInsight> EnrichAsync(string extractedText, CancellationToken cancellationToken = default)
    {
        if (!client.IsConfigured && client.UseMockFallback)
        {
            return BuildMockInsight(extractedText);
        }

        var schema = new Dictionary<string, object?>
        {
            ["type"] = "object",
            ["additionalProperties"] = false,
            ["properties"] = new Dictionary<string, object?>
            {
                ["title"] = new Dictionary<string, object?> { ["type"] = "string" },
                ["summary"] = new Dictionary<string, object?> { ["type"] = "string" }
            },
            ["required"] = new[] { "title", "summary" }
        };

        const string systemPrompt = "You are a precise CV summarizer. Return only valid JSON matching the provided schema. Be concise and deterministic.";
        var userPrompt = $@"CV text:

{extractedText}";

        var response = await client.GetStructuredResponseAsync<CandidateProfileInsightContract>("candidate_profile_insight", schema, systemPrompt, userPrompt, cancellationToken);
        return new AiProfileInsight(response.Title.Trim(), response.Summary.Trim());
    }

    private static AiProfileInsight BuildMockInsight(string extractedText)
    {
        var title = extractedText.Contains("React", StringComparison.OrdinalIgnoreCase)
            ? "Full stack .NET / React profile"
            : "Software engineering profile";

        var summary = extractedText.Length > 220
            ? extractedText[..220].Trim() + "..."
            : extractedText.Trim();

        return new AiProfileInsight(title, summary);
    }

    private sealed record CandidateProfileInsightContract([property: JsonPropertyName("title")] string Title, [property: JsonPropertyName("summary")] string Summary);
}
