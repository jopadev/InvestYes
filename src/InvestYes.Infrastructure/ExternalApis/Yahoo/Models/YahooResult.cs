using InvestYes.Infrastructure.ExternalServices.Brapi;

namespace InvestYes.Infrastructure.ExternalApis.Yahoo.Models;

public sealed class YahooResult
{
    public YahooMeta Meta { get; set; } = new();
}
