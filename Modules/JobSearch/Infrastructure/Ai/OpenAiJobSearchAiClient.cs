using GrailJobApi.Modules.JobSearch.Application;
using GrailJobApi.Shared.Ai;

namespace GrailJobApi.Modules.JobSearch.Infrastructure.Ai;

public sealed class OpenAiJobSearchAiClient(OpenAiStructuredChatClient client) : IJobSearchAiClient
{
    public async Task<IReadOnlyList<JobSearchAiResult>> ExecuteAsync(string candidateProfileText, IReadOnlyList<string> criteria, IReadOnlyList<string> filteredCompanyNames, CancellationToken cancellationToken = default)
    {
        if (!client.IsConfigured && client.UseMockFallback)
        {
            return BuildMockResults(criteria, filteredCompanyNames);
        }

        var schema = new Dictionary<string, object?>
        {
            ["type"] = "object",
            ["additionalProperties"] = false,
            ["properties"] = new Dictionary<string, object?>
            {
                ["companies"] = new Dictionary<string, object?>
                {
                    ["type"] = "array",
                    ["items"] = new Dictionary<string, object?>
                    {
                        ["type"] = "object",
                        ["additionalProperties"] = false,
                        ["properties"] = new Dictionary<string, object?>
                        {
                            ["companyName"] = new Dictionary<string, object?> { ["type"] = "string" },
                            ["jobTitle"] = new Dictionary<string, object?> { ["type"] = "string" },
                            ["offerUrl"] = new Dictionary<string, object?> { ["type"] = "string" },
                            ["offerDescription"] = new Dictionary<string, object?> { ["type"] = "string" },
                            ["location"] = new Dictionary<string, object?> { ["type"] = "string" },
                            ["workMode"] = new Dictionary<string, object?> { ["type"] = "string", ["enum"] = new[] { "Unknown", "OnSite", "Hybrid", "Remote" } },
                            ["salary"] = new Dictionary<string, object?> { ["type"] = "string" },
                            ["techStack"] = new Dictionary<string, object?> { ["type"] = "string" },
                            ["matchExplanation"] = new Dictionary<string, object?> { ["type"] = "string" },
                            ["relevanceScore"] = new Dictionary<string, object?> { ["type"] = "integer", ["minimum"] = 0, ["maximum"] = 100 }
                        },
                        ["required"] = new[] { "companyName", "jobTitle", "offerUrl", "offerDescription", "location", "workMode", "salary", "techStack", "matchExplanation", "relevanceScore" }
                    }
                }
            },
            ["required"] = new[] { "companies" }
        };

        var systemPrompt = "Vous êtes un moteur déterministe de recherche d'offres d'emploi. Utilisez le texte du profil candidat, les critères de recherche et la liste des entreprises filtrées. Ne renvoyez jamais une entreprise présente dans la liste filtered_company_names. Retournez uniquement du JSON conforme au schéma. N'inventez pas de détails impossibles et fournissez des valeurs concises et professionnelles pour les champs requis. Répondez uniquement en français pour tous les champs textuels.";
        var userPayload = JsonSerializer.Serialize(new
        {
            cv_text = candidateProfileText,
            criteria,
            filtered_company_names = filteredCompanyNames,
            instructions = new
            {
                relevance_score_range = "0 to 100",
                output_language = "French",
                include_offer_url = true,
                include_location = true,
                include_work_mode = true,
                include_salary = true,
                include_tech_stack = true,
                include_match_explanation = true
            }
        });

        var response = await client.GetStructuredResponseAsync<JobSearchContract>("job_search_results", schema, systemPrompt, userPayload, cancellationToken);
        var nowUtc = DateTime.UtcNow;

        return response.Companies
            .Where(x => !string.IsNullOrWhiteSpace(x.CompanyName))
            .Where(x => !filteredCompanyNames.Contains(x.CompanyName, StringComparer.Ordinal))
            .GroupBy(x => x.CompanyName, StringComparer.Ordinal)
            .Select(group => group.OrderByDescending(x => x.RelevanceScore).First())
            .OrderByDescending(x => x.RelevanceScore)
            .Select(x => JobSearchAiResult.Create(x.CompanyName.Trim(), x.JobTitle.Trim(), x.OfferUrl.Trim(), x.OfferDescription.Trim(), x.Location.Trim(), ParseWorkMode(x.WorkMode), x.Salary.Trim(), x.TechStack.Trim(), x.MatchExplanation.Trim(), Math.Clamp(x.RelevanceScore, 0, 100), string.Empty, nowUtc))
            .ToArray();
    }

    private static IReadOnlyList<JobSearchAiResult> BuildMockResults(IReadOnlyList<string> criteria, IReadOnlyList<string> filteredCompanyNames)
    {
        var now = DateTime.UtcNow;
        var candidates = new[]
        {
            JobSearchAiResult.Create("Acme", "Développeur Full Stack Senior", "https://example.com/jobs/123", "Poste orienté produit avec une stack web moderne.", "Paris", WorkMode.Hybrid, "55k-65k", ".NET 10, React, TypeScript, PostgreSQL", "Bon alignement avec les critères .NET et React, mode hybride.", 87, string.Empty, now.AddMinutes(-3)),
            JobSearchAiResult.Create("Northwind", "Ingénieur Backend .NET", "https://example.com/jobs/456", "Rôle backend moderne avec APIs et services cloud.", "Lyon", WorkMode.Remote, "50k-60k", ".NET 10, ASP.NET Core, PostgreSQL", "Correspond bien aux critères backend avec possibilité de télétravail.", 81, string.Empty, now.AddMinutes(-2)),
            JobSearchAiResult.Create("Adventure Works", "Ingénieur Front-end plateforme", "https://example.com/jobs/789", "Poste React et TypeScript au sein d'une équipe produit.", "Nantes", WorkMode.Hybrid, "48k-58k", "React, TypeScript, Vite, Design Systems", "Opportunité front solide en adéquation avec React et TypeScript.", 78, string.Empty, now.AddMinutes(-1))
        };

        return candidates
            .Where(x => !filteredCompanyNames.Contains(x.CompanyName, StringComparer.Ordinal))
            .ToArray();
    }

    private static WorkMode ParseWorkMode(string value)
        => Enum.TryParse<WorkMode>(value, ignoreCase: true, out var workMode) ? workMode : WorkMode.Unknown;

    private sealed record JobSearchContract([property: JsonPropertyName("companies")] JobSearchCompanyContract[] Companies);

    private sealed record JobSearchCompanyContract(
        [property: JsonPropertyName("companyName")] string CompanyName,
        [property: JsonPropertyName("jobTitle")] string JobTitle,
        [property: JsonPropertyName("offerUrl")] string OfferUrl,
        [property: JsonPropertyName("offerDescription")] string OfferDescription,
        [property: JsonPropertyName("location")] string Location,
        [property: JsonPropertyName("workMode")] string WorkMode,
        [property: JsonPropertyName("salary")] string Salary,
        [property: JsonPropertyName("techStack")] string TechStack,
        [property: JsonPropertyName("matchExplanation")] string MatchExplanation,
        [property: JsonPropertyName("relevanceScore")] int RelevanceScore);
}
