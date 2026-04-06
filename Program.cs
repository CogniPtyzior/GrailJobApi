using GrailJobApi.Modules.CandidateProfile.Application;
using GrailJobApi.Modules.CandidateProfile.Infrastructure.Ai;
using GrailJobApi.Modules.CandidateProfile.Infrastructure.Pdf;
using GrailJobApi.Modules.CandidateProfile.Infrastructure.Persistence;
using GrailJobApi.Modules.CompanyWorkspace.Application;
using GrailJobApi.Modules.CompanyWorkspace.Infrastructure.Persistence;
using GrailJobApi.Modules.JobSearch.Application;
using GrailJobApi.Modules.JobSearch.Infrastructure.Ai;
using GrailJobApi.Modules.JobSearch.Infrastructure.Persistence;
using GrailJobApi.Modules.UserAccess.Application;
using GrailJobApi.Modules.UserAccess.Infrastructure.Persistence;
using GrailJobApi.Shared.Ai;
using GrailJobApi.Shared.Configuration;
using GrailJobApi.Shared.OpenApi;
using GrailJobApi.Shared.Persistence;
using GrailJobApi.Shared.Seeding;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddJsonFile(
        "appsettings.Development.local.json",
        optional: true,
        reloadOnChange: true);
}

builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.SectionName));
builder.Services.Configure<CandidateProfileOptions>(builder.Configuration.GetSection(CandidateProfileOptions.SectionName));
builder.Services.Configure<SearchOptions>(builder.Configuration.GetSection(SearchOptions.SectionName));
builder.Services.Configure<CompanyWorkspaceOptions>(builder.Configuration.GetSection(CompanyWorkspaceOptions.SectionName));
builder.Services.Configure<SeedOptions>(builder.Configuration.GetSection(SeedOptions.SectionName));
builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection(OpenAiOptions.SectionName));

var connectionString = DatabaseConnectionStringBuilder.Build(builder.Configuration);

builder.Services.AddProblemDetails();
builder.Services.AddHttpContextAccessor();
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddGrailJobSwagger();

builder.Services.AddDbContext<UserAccessDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", DbSchemas.UserAccess)));

builder.Services.AddDbContext<CandidateProfileDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", DbSchemas.CandidateProfile)));

builder.Services.AddDbContext<JobSearchDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", DbSchemas.JobSearch)));

builder.Services.AddDbContext<CompanyWorkspaceDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", DbSchemas.CompanyWorkspace)));

builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.User.RequireUniqueEmail = true;
    })
    .AddSignInManager<SignInManager<User>>()
    .AddEntityFrameworkStores<UserAccessDbContext>()
    .AddDefaultTokenProviders();

builder.Services
    .AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.Cookie.Name = "GrailJob.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            },
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddHttpClient<OpenAiStructuredChatClient>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CandidateProfileService>();
builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<CompanyWorkspaceService>();

builder.Services.AddScoped<ICandidateProfileRepository, CandidateProfileRepository>();
builder.Services.AddScoped<ISearchSessionRepository, SearchSessionRepository>();
builder.Services.AddScoped<IJobOpportunityRepository, JobOpportunityRepository>();
builder.Services.AddScoped<IPdfTextExtractor, PdfPigTextExtractor>();
builder.Services.AddScoped<ICandidateProfileAiEnricher, OpenAiCandidateProfileAiEnricher>();
builder.Services.AddScoped<IJobSearchAiClient, OpenAiJobSearchAiClient>();
builder.Services.AddScoped<DevelopmentDataSeeder>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await MigrationRunner.ApplyMigrationsAsync(app.Services);

    using (var scope = app.Services.CreateScope())
    {
        await scope.ServiceProvider.GetRequiredService<DevelopmentDataSeeder>().SeedAsync();
    }

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "GrailJob API v1");
        options.DocumentTitle = "GrailJob API";
    });
}

app.UseExceptionHandler();
app.UseForwardedHeaders();

app.MapStaticAssets();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
