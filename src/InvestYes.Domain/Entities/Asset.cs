using InvestYes.Domain.Enums;

namespace InvestYes.Domain.Entities
{
    public class Asset
    {
        public Guid AssetId { get; set; }

        public string Ticker { get; set; } = "";

        public string Name { get; set; } = "";

        public AssetType Type { get; set; }

        public decimal CurrentPrice { get; set; }

        public decimal DividendYield { get; set; }

        public decimal PVP { get; set; }

        public decimal Liquidity { get; set; }

        public DateTime CreatedAt { get; set; }

        public void UpdatePrice(decimal price)
        {
            CurrentPrice = price;
        }
    }
}
