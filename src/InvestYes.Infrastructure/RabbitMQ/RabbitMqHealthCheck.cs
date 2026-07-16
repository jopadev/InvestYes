using InvestYes.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.Messaging.Consumers;

//using Microsoft.Extensions.Diagnostics.HealthChecks;


public sealed class RabbitMqHealthCheck
    : IHealthCheck
{
    private readonly IRabbitMqConnection _connection;
    private readonly RabbitMqOptions _options;


    public RabbitMqHealthCheck(
        IRabbitMqConnection connection,
        IOptions<RabbitMqOptions> options)
    {
        _connection = connection;
        _options = options.Value;
    }


    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var healthy =
                await _connection
                    .HealthCheckAsync(
                        cancellationToken);


            if (!healthy)
            {
                return HealthCheckResult.Unhealthy(
                    "RabbitMQ indisponível");
            }


            return HealthCheckResult.Healthy(
                "RabbitMQ disponível",
                new Dictionary<string, object>
                {
                    ["host"] = _options.HostName,

                    ["port"] = _options.Port,

                    ["virtualHost"] =
                        _options.VirtualHost,

                    ["connected"] = true,

                    ["checkedAt"] =
                        DateTime.UtcNow
                });
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Erro verificando RabbitMQ",
                ex);
        }
    }
}