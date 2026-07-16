using InvestYes.Domain.Services;
using InvestYes.Infrastructure.ExternalApis.Yahoo;
using InvestYes.Infrastructure.ExternalServices.Brapi;
using Microsoft.Extensions.DependencyInjection;

namespace InvestYes.Infrastructure.ExternalApis;

public class MarketDataFactory : IMarketDataFactory
{
    private readonly IServiceProvider _provider;

    public MarketDataFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    public IMarketDataProvider GetProvider()
    {
        return _provider.GetRequiredService<YahooFinanceMarketDataProvider>();
    }

}