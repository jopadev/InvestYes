

namespace InvestYes.Application.DTOs
{
    public sealed class MarketAssetDto
    {
        public decimal Price { get; init; }

        public decimal DividendYield { get; init; }

        public decimal Pvp { get; private set; }

        public decimal Liquidity { get; private set;}

        public decimal Vacancy { get; private set; }

        internal void SetPvp(decimal v)
        {
            Pvp = v;
        }

        internal void SetVacancy(decimal v)
        {
            Vacancy = v;
        }

        internal void SetLiquidity(decimal v)
        {
            Liquidity = v;
        }
    }
}
