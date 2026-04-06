using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;

namespace GrailJobApi.Shared.OpenApi;

public static class SwaggerExtensions
{
    public static IServiceCollection AddGrailJobSwagger(this IServiceCollection services)
    {
        services.AddSwaggerExamplesFromAssemblyOf<Program>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "GrailJob API",
                Version = "v1",
                Description = "ASP.NET Core backend for GrailJob."
            });

            var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
            }

            options.ExampleFilters();
            options.SupportNonNullableReferenceTypes();
            options.UseInlineDefinitionsForEnums();
        });

        return services;
    }
}
