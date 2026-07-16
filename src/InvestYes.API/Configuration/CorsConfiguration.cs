namespace InvestYes.API.Configuration;

public static class CorsConfiguration
{
    public const string PolicyName = "InvestYesCorsPolicy";

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services,IConfiguration configuration)
    {
        var origins = configuration
            .GetSection("Cors:AllowedOrigins")
            .Get<string[]>() ?? [];

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                if (origins.Length == 0)
                {
                    // Desenvolvimento
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
                else
                {
                    // Produção
                    policy
                        .WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsConfiguration(
        this IApplicationBuilder app)
    {
        app.UseCors(PolicyName);

        return app;
    }
}
