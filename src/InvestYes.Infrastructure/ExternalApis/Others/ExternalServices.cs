namespace InvestYes.Infrastructure.ExternalApis;

public class MarketDataOptions
{
    public string Provider { get; set; } = "BRAPI";


    public BrapiOptions Brapi { get; set; } = new();


    public YahooOptions YahooFinance { get; set; } = new();
}


public class BrapiOptions
{
    public string BaseUrl { get; set; } = default!;

    public string? Token { get; set; }
}


public class YahooOptions
{
    public string BaseUrl { get; set; } = default!;
}
