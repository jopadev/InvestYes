namespace InvestYes.Domain.ValueObjects;

public sealed class MarketAsset
{
    public string Symbol { get; set; } = default!;

    public string Name { get; set; } = default!;


    public decimal Price { get; set; }


    public decimal ChangePercent { get; set; }


    public decimal DividendYield { get; set; }


    public decimal MarketCap { get; set; }


    public DateTime UpdatedAt { get; set; }

    public decimal PriceToNav { get; set; }

    public decimal BookValuePerShare { get; set; }

    public decimal FfoYield { get; set; }

    public decimal DividendYield12M { get; set; }

    public decimal Volume { get; set; }

}

public sealed class FiiScore
{
    public string Symbol { get; init; } = "";

    public decimal Score { get; init; }

    public string Classification { get; init; } = "";

    public List<string> Reasons { get; init; } = [];
}

