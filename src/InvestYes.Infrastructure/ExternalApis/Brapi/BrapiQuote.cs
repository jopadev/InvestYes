namespace InvestYes.Infrastructure.ExternalServices.Brapi;

public class BrapiQuote
{
    public string Symbol { get; set; } = "";

    public string LongName { get; set; } = "";

    public decimal RegularMarketPrice { get; set; }

    public decimal RegularMarketChangePercent { get; set; }
}
