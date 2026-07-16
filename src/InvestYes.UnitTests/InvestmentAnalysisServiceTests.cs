using AutoMapper;
using FluentAssertions;
using InvestYes.Application.DTOs;
using InvestYes.Domain.Interfaces.Repositories;
using InvestYes.Domain.Services;
using InvestYes.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace InvestYes.UnitTests
{
    public class InvestmentAnalysisServiceTests
    {
        private readonly Mock<IMarketDataProvider> _provider;
        private readonly Mock<IAssetRepository> _repository;
        private readonly Mock<IMemoryCache> _cache;
        private readonly Mock<ILogger<InvestmentAnalysisService>> _logger;
        private readonly Mock<IMapper> _mapper;

        private readonly InvestmentAnalysisService _service;

        public InvestmentAnalysisServiceTests()
        {
            _provider = new Mock<IMarketDataProvider>();
            _repository = new Mock<IAssetRepository>();
            _cache = new Mock<IMemoryCache>();
            _logger = new Mock<ILogger<InvestmentAnalysisService>>();
            _mapper = new Mock<IMapper>();
            _service = new InvestmentAnalysisService(_repository.Object,
                _provider.Object, _cache.Object, _logger.Object, _mapper.Object);
        }

        [Fact]
        public async Task Should_Return_Score()
        {
            _provider.Setup(x =>
                x.GetAssetAsync(
                    "MXRF11",
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MarketAsset
                {
                    Symbol = "MXRF11",
                    Price = 10,
                    MarketCap = 0.91m,
                    DividendYield = 12m,
                    PriceToNav = 9.91m,
                    Volume = 300000
                });

            var result =
                await _service.AnalyzeAsync(
                    "MXRF11");

            result.Score.Should().BeLessThan(70);
        }
    }
}
