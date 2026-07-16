using Serilog;

namespace InvestYes.API.Configuration;

public static class SerilogConfiguration
{
    public static IHostBuilder AddSerilogConfiguration(this IHostBuilder host)
    {
        host.UseSerilog(
            (context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)

                    .ReadFrom.Services(services)

                    .Enrich.FromLogContext()

                    .Enrich.WithProperty(
                        "Application",
                        "InvestYes.API");
            });

        return host;
    }
}

