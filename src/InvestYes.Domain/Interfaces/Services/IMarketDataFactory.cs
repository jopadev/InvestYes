namespace InvestYes.Domain.Services;

public interface IMarketDataFactory
{
    IMarketDataProvider GetProvider();
}