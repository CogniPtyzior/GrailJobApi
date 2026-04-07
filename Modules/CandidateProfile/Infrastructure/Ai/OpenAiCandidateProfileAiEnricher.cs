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
                ["title"] = CreateStringSchema(200),
                ["summary"] = CreateStringSchema(1600),
                ["targetRoles"] = CreateStringArraySchema(6, 120),
                ["preferredJobTitles"] = CreateStringArraySchema(6, 120),
                ["coreSkills"] = CreateStringArraySchema(10, 80),
                ["secondarySkills"] = CreateStringArraySchema(10, 80),
                ["mustHaveSkills"] = CreateStringArraySchema(8, 80),
                ["niceToHaveSkills"] = CreateStringArraySchema(8, 80),
                ["domains"] = CreateStringArraySchema(8, 100),
                ["industries"] = CreateStringArraySchema(8, 100),
                ["companyTypes"] = CreateStringArraySchema(6, 80),
                ["experienceHighlights"] = CreateStringArraySchema(6, 220),
                ["architectureFocus"] = CreateStringArraySchema(8, 100),
                ["deliveryPractices"] = CreateStringArraySchema(8, 100),
                ["languagesSpoken"] = CreateStringArraySchema(6, 50),
                ["workModes"] = CreateStringArraySchema(4, 50),
                ["locations"] = CreateStringArraySchema(8, 100),
                ["mobilityArea"] = CreateStringArraySchema(6, 100),
                ["seniority"] = CreateStringSchema(80),
                ["experienceLevelYears"] = CreateStringSchema(80),
                ["managementScope"] = CreateStringSchema(120),
                ["certifications"] = CreateStringArraySchema(8, 120),
                ["contractPreferences"] = CreateStringArraySchema(6, 80),
                ["searchKeywords"] = CreateStringArraySchema(12, 80),
                ["personalityTraits"] = CreateStringArraySchema(5, 80),
                ["softSkills"] = CreateStringArraySchema(6, 80),
                ["educationDetails"] = CreateStringArraySchema(6, 160),
                ["hobbies"] = CreateStringArraySchema(6, 80)
            },
            ["required"] = new[]
            {
                "title",
                "summary",
                "targetRoles",
                "preferredJobTitles",
                "coreSkills",
                "secondarySkills",
                "mustHaveSkills",
                "niceToHaveSkills",
                "domains",
                "industries",
                "companyTypes",
                "experienceHighlights",
                "architectureFocus",
                "deliveryPractices",
                "languagesSpoken",
                "workModes",
                "locations",
                "mobilityArea",
                "seniority",
                "experienceLevelYears",
                "managementScope",
                "certifications",
                "contractPreferences",
                "searchKeywords",
                "personalityTraits",
                "softSkills",
                "educationDetails",
                "hobbies"
            }
        };

        const string systemPrompt =
            "Vous êtes un moteur déterministe de synthèse de CV. " +
            "Retournez uniquement un JSON valide conforme au schéma fourni. " +
            "Rédigez impérativement tous les champs textuels en français. " +
            "Le titre doit être court, professionnel et en français. " +
            "Le résumé doit être concis, factuel, fluide et en français. " +
            "Les listes doivent être courtes, utiles, non redondantes et strictement fondées sur le CV. " +
            "N'inventez aucune information absente du CV. " +
            "Les champs personalityTraits, softSkills, educationDetails et hobbies doivent rester synthétiques et secondaires.";

        var userPrompt = $"""
CV à analyser :

{extractedText}

Consignes :
- produire `title` en français
- produire `summary` en français
- produire des listes courtes et utiles
- ne pas recopier le CV mot à mot
- ne retourner que le JSON demandé
""";

        var response = await client.GetStructuredResponseAsync<CandidateProfileInsightContract>(
            "candidate_profile_insight",
            schema,
            systemPrompt,
            userPrompt,
            cancellationToken);

        return new AiProfileInsight(
            response.Title.Trim(),
            response.Summary.Trim(),
            targetRoles: response.TargetRoles,
            preferredJobTitles: response.PreferredJobTitles,
            coreSkills: response.CoreSkills,
            secondarySkills: response.SecondarySkills,
            mustHaveSkills: response.MustHaveSkills,
            niceToHaveSkills: response.NiceToHaveSkills,
            domains: response.Domains,
            industries: response.Industries,
            companyTypes: response.CompanyTypes,
            experienceHighlights: response.ExperienceHighlights,
            architectureFocus: response.ArchitectureFocus,
            deliveryPractices: response.DeliveryPractices,
            languagesSpoken: response.LanguagesSpoken,
            workModes: response.WorkModes,
            locations: response.Locations,
            mobilityArea: response.MobilityArea,
            seniority: response.Seniority,
            experienceLevelYears: response.ExperienceLevelYears,
            managementScope: response.ManagementScope,
            certifications: response.Certifications,
            contractPreferences: response.ContractPreferences,
            searchKeywords: response.SearchKeywords,
            personalityTraits: response.PersonalityTraits,
            softSkills: response.SoftSkills,
            educationDetails: response.EducationDetails,
            hobbies: response.Hobbies);
    }

    private static AiProfileInsight BuildMockInsight(string extractedText)
    {
        var isFullStack = extractedText.Contains("React", StringComparison.OrdinalIgnoreCase);

        return new AiProfileInsight(
            title: isFullStack ? "Profil full stack .NET / React" : "Profil ingénierie logicielle",
            summary: extractedText.Length > 220
                ? extractedText[..220].Trim() + "..."
                : extractedText.Trim(),
            targetRoles: isFullStack ? ["Développeur full stack", "Ingénieur logiciel"] : ["Ingénieur logiciel", "Développeur backend"],
            preferredJobTitles: isFullStack ? [".NET Full Stack Developer", "Software Engineer"] : ["Backend .NET Developer", "Software Engineer"],
            coreSkills: [".NET", "C#", "API REST"],
            secondarySkills: isFullStack ? ["React", "TypeScript", "PostgreSQL"] : ["Docker", "Azure", "PostgreSQL"],
            mustHaveSkills: [".NET", "C#"],
            niceToHaveSkills: ["Azure", "CI/CD"],
            domains: ["Développement logiciel", "Applications métiers"],
            industries: ["SaaS", "Services numériques"],
            companyTypes: ["Éditeur", "ESN", "Scale-up"],
            experienceHighlights: ["Expérience confirmée sur le développement d'applications web et backend.", "Capacité à intervenir sur des environnements cloud et distribués."],
            architectureFocus: ["Microservices", "API", "Architecture distribuée"],
            deliveryPractices: ["CI/CD", "Automatisation", "Observabilité"],
            languagesSpoken: ["Français", "Anglais"],
            workModes: ["Hybrid", "Remote"],
            locations: ["Paris", "Lyon"],
            mobilityArea: ["France"],
            seniority: "Senior",
            experienceLevelYears: "10+ ans",
            managementScope: "Lead technique",
            certifications: ["Azure Fundamentals"],
            contractPreferences: ["CDI"],
            searchKeywords: [".NET", "C#", "Azure", "React", "API"],
            personalityTraits: ["rigoureux", "autonome", "pragmatique"],
            softSkills: ["communication", "travail en équipe", "mentorat"],
            educationDetails: ["Formation supérieure en informatique"],
            hobbies: ["veille technologique", "open source"]);
    }

    private static Dictionary<string, object?> CreateStringSchema(int maxLength) => new()
    {
        ["type"] = "string",
        ["maxLength"] = maxLength
    };

    private static Dictionary<string, object?> CreateStringArraySchema(int maxItems, int maxItemLength) => new()
    {
        ["type"] = "array",
        ["items"] = CreateStringSchema(maxItemLength),
        ["maxItems"] = maxItems
    };

    private sealed record CandidateProfileInsightContract(
        [property: JsonPropertyName("title")] string Title,
        [property: JsonPropertyName("summary")] string Summary,
        [property: JsonPropertyName("targetRoles")] string[] TargetRoles,
        [property: JsonPropertyName("preferredJobTitles")] string[] PreferredJobTitles,
        [property: JsonPropertyName("coreSkills")] string[] CoreSkills,
        [property: JsonPropertyName("secondarySkills")] string[] SecondarySkills,
        [property: JsonPropertyName("mustHaveSkills")] string[] MustHaveSkills,
        [property: JsonPropertyName("niceToHaveSkills")] string[] NiceToHaveSkills,
        [property: JsonPropertyName("domains")] string[] Domains,
        [property: JsonPropertyName("industries")] string[] Industries,
        [property: JsonPropertyName("companyTypes")] string[] CompanyTypes,
        [property: JsonPropertyName("experienceHighlights")] string[] ExperienceHighlights,
        [property: JsonPropertyName("architectureFocus")] string[] ArchitectureFocus,
        [property: JsonPropertyName("deliveryPractices")] string[] DeliveryPractices,
        [property: JsonPropertyName("languagesSpoken")] string[] LanguagesSpoken,
        [property: JsonPropertyName("workModes")] string[] WorkModes,
        [property: JsonPropertyName("locations")] string[] Locations,
        [property: JsonPropertyName("mobilityArea")] string[] MobilityArea,
        [property: JsonPropertyName("seniority")] string Seniority,
        [property: JsonPropertyName("experienceLevelYears")] string ExperienceLevelYears,
        [property: JsonPropertyName("managementScope")] string ManagementScope,
        [property: JsonPropertyName("certifications")] string[] Certifications,
        [property: JsonPropertyName("contractPreferences")] string[] ContractPreferences,
        [property: JsonPropertyName("searchKeywords")] string[] SearchKeywords,
        [property: JsonPropertyName("personalityTraits")] string[] PersonalityTraits,
        [property: JsonPropertyName("softSkills")] string[] SoftSkills,
        [property: JsonPropertyName("educationDetails")] string[] EducationDetails,
        [property: JsonPropertyName("hobbies")] string[] Hobbies);
}