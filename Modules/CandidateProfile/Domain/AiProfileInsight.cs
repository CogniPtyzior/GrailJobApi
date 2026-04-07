namespace GrailJobApi.Modules.CandidateProfile.Domain;

public sealed class AiProfileInsight
{
    public string Title { get; private set; } = string.Empty;
    public string Summary { get; private set; } = string.Empty;

    public string[] TargetRoles { get; private set; } = [];
    public string[] PreferredJobTitles { get; private set; } = [];
    public string[] CoreSkills { get; private set; } = [];
    public string[] SecondarySkills { get; private set; } = [];
    public string[] MustHaveSkills { get; private set; } = [];
    public string[] NiceToHaveSkills { get; private set; } = [];
    public string[] Domains { get; private set; } = [];
    public string[] Industries { get; private set; } = [];
    public string[] CompanyTypes { get; private set; } = [];
    public string[] ExperienceHighlights { get; private set; } = [];
    public string[] ArchitectureFocus { get; private set; } = [];
    public string[] DeliveryPractices { get; private set; } = [];
    public string[] LanguagesSpoken { get; private set; } = [];
    public string[] WorkModes { get; private set; } = [];
    public string[] Locations { get; private set; } = [];
    public string[] MobilityArea { get; private set; } = [];
    public string[] Certifications { get; private set; } = [];
    public string[] ContractPreferences { get; private set; } = [];
    public string[] SearchKeywords { get; private set; } = [];
    public string[] PersonalityTraits { get; private set; } = [];
    public string[] SoftSkills { get; private set; } = [];
    public string[] EducationDetails { get; private set; } = [];
    public string[] Hobbies { get; private set; } = [];

    public string Seniority { get; private set; } = string.Empty;
    public string ExperienceLevelYears { get; private set; } = string.Empty;
    public string ManagementScope { get; private set; } = string.Empty;

    private AiProfileInsight()
    {
    }

    public AiProfileInsight(
        string title,
        string summary,
        IEnumerable<string>? targetRoles = null,
        IEnumerable<string>? preferredJobTitles = null,
        IEnumerable<string>? coreSkills = null,
        IEnumerable<string>? secondarySkills = null,
        IEnumerable<string>? mustHaveSkills = null,
        IEnumerable<string>? niceToHaveSkills = null,
        IEnumerable<string>? domains = null,
        IEnumerable<string>? industries = null,
        IEnumerable<string>? companyTypes = null,
        IEnumerable<string>? experienceHighlights = null,
        IEnumerable<string>? architectureFocus = null,
        IEnumerable<string>? deliveryPractices = null,
        IEnumerable<string>? languagesSpoken = null,
        IEnumerable<string>? workModes = null,
        IEnumerable<string>? locations = null,
        IEnumerable<string>? mobilityArea = null,
        string? seniority = null,
        string? experienceLevelYears = null,
        string? managementScope = null,
        IEnumerable<string>? certifications = null,
        IEnumerable<string>? contractPreferences = null,
        IEnumerable<string>? searchKeywords = null,
        IEnumerable<string>? personalityTraits = null,
        IEnumerable<string>? softSkills = null,
        IEnumerable<string>? educationDetails = null,
        IEnumerable<string>? hobbies = null)
    {
        Title = NormalizeText(title);
        Summary = NormalizeText(summary);

        TargetRoles = NormalizeItems(targetRoles, 6);
        PreferredJobTitles = NormalizeItems(preferredJobTitles, 6);
        CoreSkills = NormalizeItems(coreSkills, 10);
        SecondarySkills = NormalizeItems(secondarySkills, 10);
        MustHaveSkills = NormalizeItems(mustHaveSkills, 8);
        NiceToHaveSkills = NormalizeItems(niceToHaveSkills, 8);
        Domains = NormalizeItems(domains, 8);
        Industries = NormalizeItems(industries, 8);
        CompanyTypes = NormalizeItems(companyTypes, 6);
        ExperienceHighlights = NormalizeItems(experienceHighlights, 6);
        ArchitectureFocus = NormalizeItems(architectureFocus, 8);
        DeliveryPractices = NormalizeItems(deliveryPractices, 8);
        LanguagesSpoken = NormalizeItems(languagesSpoken, 6);
        WorkModes = NormalizeItems(workModes, 4);
        Locations = NormalizeItems(locations, 8);
        MobilityArea = NormalizeItems(mobilityArea, 6);
        Certifications = NormalizeItems(certifications, 8);
        ContractPreferences = NormalizeItems(contractPreferences, 6);
        SearchKeywords = NormalizeItems(searchKeywords, 12);
        PersonalityTraits = NormalizeItems(personalityTraits, 5);
        SoftSkills = NormalizeItems(softSkills, 6);
        EducationDetails = NormalizeItems(educationDetails, 6);
        Hobbies = NormalizeItems(hobbies, 6);

        Seniority = NormalizeText(seniority);
        ExperienceLevelYears = NormalizeText(experienceLevelYears);
        ManagementScope = NormalizeText(managementScope);
    }

    private static string NormalizeText(string? value)
        => value?.Trim() ?? string.Empty;

    private static string[] NormalizeItems(IEnumerable<string>? values, int maxItems)
        => values?
            .Select(x => x?.Trim() ?? string.Empty)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(maxItems)
            .ToArray()
        ?? [];
}