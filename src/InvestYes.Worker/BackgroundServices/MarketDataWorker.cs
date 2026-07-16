using InvestYes.Domain.Interfaces.Repositories;
using InvestYes.Domain.Services;
using InvestYes.Infrastructure.Observability;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InvestYes.Worker.BackgroundServices;

public sealed class MarketDataWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<MarketDataWorker> _logger;

    public MarketDataWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<MarketDataWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var repository =
                    scope.ServiceProvider.GetRequiredService<IAssetRepository>();

                var provider =
                    scope.ServiceProvider.GetRequiredService<IMarketDataProvider>();

                var assets =
                    await repository.GetAllAsync(stoppingToken);

                foreach (var asset in assets)
                {
                    var market =
                        await provider.GetAssetAsync(asset.Ticker, stoppingToken);

                    if (market is null)
                        continue;

                    asset.UpdatePrice(market.Price);

                    await repository.UpdateAsync(asset, stoppingToken);

                    _logger.LogInformation("Atualizando {Quantidade} ativos.",assets.Count);

                    using var activity =
                        Telemetry.ActivitySource.StartActivity("UpdateAssetPrice");

                    activity?.SetTag("asset.symbol", asset.Ticker);
                    activity?.SetTag("provider", provider.ProviderName);

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no MarketDataWorker.");
            }

            await Task.Delay(
                TimeSpan.FromMinutes(5),
                stoppingToken);
        }
    }
}

