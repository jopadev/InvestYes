using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace InvestYes.API.Configuration;

public static class OpenTelemetryConfiguration
{
    public static IServiceCollection AddOpenTelemetryConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                resource.AddService(
                    serviceName:
                    configuration["OpenTelemetry:ServiceName"]
                    ?? "InvestYes.API",

                    serviceVersion:
                    configuration["OpenTelemetry:ServiceVersion"]
                    ?? "1.0.0");
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                tracing.AddOtlpExporter(options =>
                {
                    options.Endpoint =
                        new Uri(
                            configuration["OpenTelemetry:OtlpEndpoint"]
                            ??
                            "http://localhost:4317");
                });
            });

        return services;
    }
}
