using Asp.Versioning.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace InvestYes.API.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "InvestYes API",
                Version = "v1",
                Description = "API para gerenciamento e análise de carteira de investimentos.",
                Contact = new OpenApiContact
                {
                    Name = "InvestYes"
                }
            });

            // XML Documentation
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath, true);
            }

            // JWT Bearer Authentication
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description =
                    "Informe o token JWT.\n\n" +
                    "Exemplo:\n" +
                    "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",

                Name = "Authorization",

                In = ParameterLocation.Header,

                Type = SecuritySchemeType.Http,

                Scheme = "bearer",

                BearerFormat = "JWT"
            });

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Id = "Bearer",
                                Type = ReferenceType.SecurityScheme
                            }
                        },
                        Array.Empty<string>()
                    }
                });

            options.EnableAnnotations();

            options.CustomSchemaIds(type => type.FullName);

            options.SupportNonNullableReferenceTypes();
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerConfiguration(this IApplicationBuilder app)
    {
        var apiVersionDescriptionProvider =
            app.ApplicationServices
            .GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            foreach (var description
        in apiVersionDescriptionProvider.ApiVersionDescriptions)
            {
                options.SwaggerEndpoint(
            $"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
            }

            options.DocumentTitle = "InvestYes API";

            options.DisplayRequestDuration();

            options.EnableDeepLinking();

            options.EnableFilter();

            options.DocExpansion(
                Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);

            options.DefaultModelsExpandDepth(-1);

            options.DefaultModelExpandDepth(2);
        });

        return app;
    }
}


