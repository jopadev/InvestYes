using FluentAssertions;
using FluentAssertions.Execution;
using InvestYes.Infrastructure.ExternalApis.Brapi.Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;

namespace InvestYes.UnitTests
{
    public class ScoreCalculatorTests
    {
        private readonly IFiiScoreService _scoreService;
        public ScoreCalculatorTests()
        {
            _scoreService = new FiiScoreService();
        }

        [Theory]
        [InlineData(0.80, 0.13, 0.01, 2_000_000_000, 2_500_000_000, 500000, "Excelente")]
        [InlineData(0.95, 0.10, 0.01, 1_500_000_000, 2_000_000_000, 300000, "Barato")]
        [InlineData(1.40, 0.07, -0.06, 200_000_000, 400_000_000, 900, "Muito Caro")]

        /*
         >= 85 => "Excelente",

            >= 70 => "Barato",

            >= 55 => "Neutro",

            >= 40 => "Caro",

            _ => "Muito Caro"
         */
        public void Should_Calculate_Score(
        decimal pvp,
        decimal dy,
        decimal monthlyReturn,
        decimal equity,
        decimal totalAssets,
        int investors,
        string classification)
        {
            var oScore = _scoreService.Calculate(new BrapiFiiIndicator
            {
                PriceToNav = pvp,
                DividendYield12m = dy,
                MonthlyReturn = monthlyReturn,
                Equity = equity,
                TotalAssets = totalAssets,
                TotalInvestors = investors
            });

            oScore.Classification.Should().Be(classification);
        }

        [Theory]
        [InlineData(95, "Compra Forte")]
        [InlineData(80, "Comprar")]
        [InlineData(65, "Neutro")]
        [InlineData(45, "Cautela")]
        [InlineData(20, "Evitar")]
        public void Should_Return_Correct_Recommendation(
            int score,
            string recommendation)
        {
            _scoreService
                .GetRecommendation(score)
                .Should()
                .Be(recommendation);
        }

        [Theory]
        [InlineData(0.80, "Barato")]
        [InlineData(1.00, "Neutro")]
        [InlineData(1.20, "Caro")]
        public void Should_Classify_Valuation(
            decimal pvp,
            string expected)
        {
            _scoreService
                .GetValuation(pvp)
                .Should()
                .Be(expected);
        }
    }
}
