namespace InvestYes.Infrastructure.ExternalApis;

public sealed class MarketDataRateLimitException : Exception
{
    public MarketDataRateLimitException(string message)
        : base(message)
    {
    }
}
