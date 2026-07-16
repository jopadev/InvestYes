
using InvestYes.Domain.ValueObjects;

namespace InvestYes.Domain.Services;

public interface IMarketDataProvider
{
    string ProviderName { get; }

    int Priority { get; }

    bool Supports(string ticker);

    Task<MarketAsset?> GetAssetAsync(
        string ticker,
        CancellationToken cancellationToken = default);


    Task<IEnumerable<MarketAsset>> GetAssetsAsync(
        IEnumerable<string> tickers,
        CancellationToken cancellationToken = default);
}
