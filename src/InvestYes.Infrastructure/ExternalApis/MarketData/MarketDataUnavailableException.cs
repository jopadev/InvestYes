namespace InvestYes.Infrastructure.ExternalApis;

public sealed class MarketDataUnavailableException : Exception
{
    public MarketDataUnavailableException(
        string provider,
        Exception? inner = null)
        : base($"O provedor {provider} está indisponível.", inner)
    {
    }
}