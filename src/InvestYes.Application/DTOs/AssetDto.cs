
namespace InvestYes.Application.DTOs
{
    public class AssetDto
    {
        public Guid AssetId { get; set; }

        public string Ticker { get; set; } = "";

        public string Name { get; set; } = "";

        public string Type { get; set; } = string.Empty;

        public decimal CurrentPrice { get; set; }

        public decimal DividendYield { get; set; }

        public decimal PVP { get; set; }

        public decimal Liquidity { get; set; }

    }
}
