namespace InvestYes.Application.DTOs
{
    public sealed class AssetAnalysisDto
    {
        public string Ticker { get; init; } = string.Empty;

        public string Name { get; init; } = string.Empty;

        public decimal CurrentPrice { get; init; }

        public decimal DividendYield { get; init; }

        public decimal Pvp { get; init; }

        public decimal Liquidity { get; init; }

        public decimal Score { get; init; }

        public string Recommendation { get; init; } = string.Empty;

        public DateTime AnalyzedAt { get; init; }
    }
}
