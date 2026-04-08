using GrailJobApi.Modules.CandidateProfile.Presentation.Responses;
using GrailJobApi.Modules.CompanyWorkspace.Presentation.Requests;
using GrailJobApi.Modules.CompanyWorkspace.Presentation.Responses;
using GrailJobApi.Modules.JobSearch.Presentation.Requests;
using GrailJobApi.Modules.JobSearch.Presentation.Responses;
using GrailJobApi.Modules.UserAccess.Presentation.Requests;
using GrailJobApi.Modules.UserAccess.Presentation.Responses;
using Swashbuckle.AspNetCore.Filters;

namespace GrailJobApi.Shared.OpenApi;

public sealed class LoginRequestExample : IExamplesProvider<LoginRequest>
{
    public LoginRequest GetExamples() => new()
    {
        Email = "test@grailjob.local",
        Password = "Test1234!"
    };
}

public sealed class LoginResponseExample : IExamplesProvider<LoginResponse>
{
    public LoginResponse GetExamples() => new(new UserResponse(
        "8a4d6d1a-6fef-4b6c-ae47-c7e6d9301e41",
        "test@grailjob.local",
        "Test User",
        "Test",
        "User",
        true,
        DateTime.Parse("2026-04-03T09:00:00Z"),
        DateTime.Parse("2026-04-08T08:30:00Z"),
        true,
        DateTime.Parse("2026-04-08T08:35:00Z"),
        DateTime.Parse("2026-04-08T08:40:00Z"),
        true));
}

public sealed class LogoutResponseExample : IExamplesProvider<LogoutResponse>
{
    public LogoutResponse GetExamples() => new(true);
}

public sealed class CvProfileResponseExample : IExamplesProvider<CvProfileResponse>
{
    public CvProfileResponse GetExamples() => new(
        true,
        DateTime.Parse("2026-04-03T10:15:00Z"),
        "Full stack .NET / React profile",
        "Experienced developer with strong .NET, React and product-oriented delivery background.",
        "cv_john_doe.pdf",
        ["Développeur full stack", "Ingénieur logiciel"],
        [".NET Full Stack Developer", "Software Engineer"],
        [".NET", "C#", "API REST"],
        ["React", "TypeScript", "PostgreSQL"],
        [".NET", "C#"],
        ["Azure", "CI/CD"],
        ["Développement logiciel", "Applications métiers"],
        ["SaaS", "Services numériques"],
        ["Éditeur", "ESN", "Scale-up"],
        [
            "Expérience confirmée sur le développement d'applications web et backend.",
            "Capacité à intervenir sur des environnements cloud et distribués."
        ],
        ["Microservices", "API", "Architecture distribuée"],
        ["CI/CD", "Automatisation", "Observabilité"],
        ["Français", "Anglais"],
        ["Hybrid", "Remote"],
        ["Paris", "Lyon"],
        ["France"],
        "Senior",
        "10+ ans",
        "Lead technique",
        ["Azure Fundamentals"],
        ["CDI"],
        [".NET", "C#", "Azure", "React", "API"],
        ["rigoureux", "autonome", "pragmatique"],
        ["communication", "travail en équipe", "mentorat"],
        ["Formation supérieure en informatique"],
        ["veille technologique", "open source"]);
}

public sealed class SearchCriteriaRequestExample : IExamplesProvider<SearchCriteriaRequest>
{
    public SearchCriteriaRequest GetExamples() => new()
    {
        Criteria = ["React TypeScript", ".NET 10 backend", "Hybrid work"]
    };
}

public sealed class SearchCriteriaResponseExample : IExamplesProvider<SearchCriteriaResponse>
{
    public SearchCriteriaResponse GetExamples() => new(["React TypeScript", ".NET 10 backend", "Hybrid work"]);
}

public sealed class SearchResponseExample : IExamplesProvider<SearchResponse>
{
    public SearchResponse GetExamples() => new(true,
    [
        new CompanyResponse(
            "f04f7e86-5d07-4d50-a304-dc8fb9b9d3e2",
            "Acme",
            "Senior Full Stack Developer",
            "https://example.com/jobs/123",
            "Product-oriented role with modern web stack.",
            "Paris",
            "Hybrid",
            "55k-65k",
            ".NET 10, React, TypeScript, PostgreSQL",
            "Strong match for .NET and React criteria with hybrid setup.",
            string.Empty,
            "result",
            "2026-04-03T10:40:00.0000000Z")
    ]);
}

public sealed class CompanyListResponseExample : IExamplesProvider<CompanyListResponse>
{
    public CompanyListResponse GetExamples() => new(
    [
        new CompanyResponse(
            "2a2b4f54-52f7-49f4-bb08-d4f8f9872c03",
            "Contoso",
            "Backend .NET Engineer",
            "https://example.com/jobs/456",
            "Product-oriented backend role with modern .NET stack.",
            "Lyon",
            "Remote",
            "55k-65k",
            ".NET 10, ASP.NET Core, PostgreSQL, React",
            "Strong alignment with the candidate profile thanks to modern .NET, React exposure, and remote flexibility.",
            "Very interesting mission and stack.",
            "saved",
            "2026-04-03T11:00:00.0000000Z")
    ]);
}

public sealed class CompanyResponseExample : IExamplesProvider<CompanyResponse>
{
    public CompanyResponse GetExamples() => new(
        "2a2b4f54-52f7-49f4-bb08-d4f8f9872c03",
        "Contoso",
        "Backend .NET Engineer",
        "https://example.com/jobs/456",
        "Product-oriented backend role with modern .NET stack.",
        "Lyon",
        "Remote",
        "55k-65k",
        ".NET 10, ASP.NET Core, PostgreSQL, React",
        "Strong alignment with the candidate profile thanks to modern .NET, React exposure, and remote flexibility.",
        "Very interesting mission and stack.",
        "saved",
        "2026-04-03T11:00:00.0000000Z");
}

public sealed class UpdateCompanyStatusRequestExample : IExamplesProvider<UpdateCompanyStatusRequest>
{
    public UpdateCompanyStatusRequest GetExamples() => new()
    {
        Status = "Excluded"
    };
}

public sealed class UpdateCompanyCommentRequestExample : IExamplesProvider<UpdateCompanyCommentRequest>
{
    public UpdateCompanyCommentRequest GetExamples() => new()
    {
        Comment = "Interesting company, but need to confirm remote policy."
    };
}
