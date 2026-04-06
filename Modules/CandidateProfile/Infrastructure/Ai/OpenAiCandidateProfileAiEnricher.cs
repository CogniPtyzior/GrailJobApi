using GrailJobApi.Modules.CandidateProfile.Application;
using GrailJobApi.Modules.CandidateProfile.Domain;
using GrailJobApi.Shared.Ai;
using System.Text.Json.Serialization;

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

        const string systemPrompt =
            "Vous êtes un moteur déterministe de synthèse de CV. " +
            "Retournez uniquement un JSON valide conforme au schéma fourni. " +
            "Rédigez impérativement tous les champs textuels en français. " +
            "Le titre doit être court, professionnel et en français. " +
            "Le résumé doit être concis, factuel, fluide et en français. " +
            "N'inventez aucune information absente du CV.";

        var userPrompt = $"""
CV à analyser :

{extractedText}

Consignes :
- produire `title` en français
- produire `summary` en français
- ne pas recopier le CV mot à mot
- ne retourner que le JSON demandé
""";

        var response = await client.GetStructuredResponseAsync<CandidateProfileInsightContract>(
            "candidate_profile_insight",
            schema,
            systemPrompt,
            userPrompt,
            cancellationToken);

        return new AiProfileInsight(response.Title.Trim(), response.Summary.Trim());
    }

    private static AiProfileInsight BuildMockInsight(string extractedText)
    {
        var title = extractedText.Contains("React", StringComparison.OrdinalIgnoreCase)
            ? "Profil full stack .NET / React"
            : "Profil ingénierie logicielle";

        var summary = extractedText.Length > 220
            ? extractedText[..220].Trim() + "..."
            : extractedText.Trim();

        return new AiProfileInsight(title, summary);
    }

    private sealed record CandidateProfileInsightContract(
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("summary")] string Summary);
}