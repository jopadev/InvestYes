namespace InvestYes.Infrastructure.ExternalApis.Yahoo.Models;

public sealed class YahooResponse
{
    public YahooChart Chart { get; set; } = new();
}
