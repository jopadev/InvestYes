using FluentAssertions;
using InvestYes.Domain.Entities;
using InvestYes.Domain.Enums;

namespace InvestYes.UnitTests
{
    public class AssetTests
    {
        [Fact]
        public void Should_Create_Asset()
        {
            var asset = new Asset
            {
                Ticker = "MXRF11",
                Type = AssetType.FII
            };

            asset.Ticker.Should().Be("MXRF11");
        }
    }
}
