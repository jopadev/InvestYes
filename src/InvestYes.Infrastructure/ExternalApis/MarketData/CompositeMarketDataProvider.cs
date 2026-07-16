using System.Diagnostics;
using InvestYes.Domain.Services;
using InvestYes.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace InvestYes.Infrastructure.ExternalApis.MarketData;

public sealed class CompositeMarketDataProvider : IMarketDataProvider
{
    private static readonly ActivitySource Activity = new("InvestYes.Infrastructure");

    private readonly ILogger<CompositeMarketDataProvider> _logger;

    private readonly IEnumerable<IMarketDataProvider> _providers;

    public string ProviderName => "Composite Provider";

    public int Priority => 0;

    public CompositeMarketDataProvider(IEnumerable<IMarketDataProvider> providers, ILogger<CompositeMarketDataProvider> logger)
    {
        _providers = providers;
        _logger = logger;
    }

    public async Task<MarketAsset?> GetAssetAsync(string symbol, CancellationToken cancellationToken = default)
    {
        using var activity = Activity.StartActivity("Composite Provider");

        activity?.SetTag("asset.symbol", symbol);

        var result = new MarketAsset
        {
            Symbol = symbol
        };

        foreach (var provider in _providers.Where(x => x is not CompositeMarketDataProvider).OrderBy(y => y.Priority))
        {
            try
            {
                _logger.LogInformation("Consultando provider {Provider}", provider.ProviderName);

                var oMarketAsset = await provider.GetAssetAsync(symbol, cancellationToken);

                if (oMarketAsset is null)
                    continue;

                Merge(result, oMarketAsset);

                _logger.LogInformation("{Provider} retornou dados.", provider.ProviderName);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Falha no provider {Provider}", provider.ProviderName);
            }
        }

        return result;
    }

    private static void Merge(MarketAsset destination, MarketAsset source)
    {
        destination.Name ??= source.Name;
        destination.Symbol ??= source.Symbol;
        destination.Price = destination.Price == 0 ? source.Price : destination.Price;
        destination.PriceToNav = destination.PriceToNav == 0 ? source.PriceToNav : destination.PriceToNav;
        destination.BookValuePerShare = destination.BookValuePerShare == 0 ? source.BookValuePerShare : destination.BookValuePerShare;
        destination.DividendYield12M = destination.DividendYield12M == 0 ? source.DividendYield12M : destination.DividendYield12M;
        destination.DividendYield = destination.DividendYield == 0 ? source.DividendYield : destination.DividendYield;
        destination.FfoYield = destination.FfoYield == 0 ? source.FfoYield : destination.FfoYield;
        destination.Volume = destination.Volume == 0 ? source.Volume : destination.Volume;
        destination.MarketCap = destination.MarketCap == 0 ? source.MarketCap : destination.MarketCap;
    }

    public bool Supports(string ticker) => ticker.EndsWith("3") || ticker.EndsWith("4") || ticker.EndsWith("11");

    public Task<IEnumerable<MarketAsset>> GetAssetsAsync(IEnumerable<string> tickers, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}