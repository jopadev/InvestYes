namespace InvestYes.Infrastructure.ExternalServices.Brapi;

public sealed class YahooMeta
{
    public string Symbol { get; set; } = string.Empty;

    public decimal RegularMarketPrice { get; set; }

    public string Currency { get; set; } = string.Empty;

    public string ExchangeName { get; set; } = string.Empty;
}