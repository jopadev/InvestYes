using Asp.Versioning;

namespace InvestYes.API.Configuration;

public static class ApiVersioningConfiguration
{
    public static IServiceCollection AddApiVersioningConfiguration(
        this IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                // Versão padrão quando não informada
                options.DefaultApiVersion =
                    new ApiVersion(1, 0);

                // Usa v1 automaticamente
                options.AssumeDefaultVersionWhenUnspecified =
                    true;

                // Retorna headers informativos
                options.ReportApiVersions = true;

                // Versão via URL
                options.ApiVersionReader =
                    new UrlSegmentApiVersionReader();
            })

            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";

                options.SubstituteApiVersionInUrl = true;
            });

        return services;
    }
}
