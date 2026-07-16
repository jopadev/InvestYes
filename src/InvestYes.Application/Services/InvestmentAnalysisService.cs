using AutoMapper;
using InvestYes.Application.DTOs;
using InvestYes.Application.Interfaces;
using InvestYes.Domain.Interfaces.Repositories;
using InvestYes.Domain.ValueObjects;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace InvestYes.Domain.Services
{
    public sealed class InvestmentAnalysisService : IInvestmentAnalysisService
    {
        private readonly IAssetRepository _repository;
        private readonly IMarketDataProvider _marketData;
        private readonly IMemoryCache _cache;
        private readonly ILogger<InvestmentAnalysisService> _logger;
        private readonly IMapper _mapper;

        public InvestmentAnalysisService(IAssetRepository repository, IMarketDataProvider marketData, IMemoryCache cache, ILogger<InvestmentAnalysisService> logger, IMapper mapper)
        {
            _repository = repository;
            _marketData = marketData;
            _cache = cache;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<AssetAnalysisDto> AnalyzeAsync(string ticker, CancellationToken cancellationToken = default)
        {
            ticker = ticker.Trim().ToUpperInvariant();

            var sCacheKey = $"analysis:{ticker}";

            if (_cache.TryGetValue(sCacheKey, out AssetAnalysisDto? cached) && cached is not null)
            {
                _logger.LogInformation("Análise encontrada no cache: {Ticker}", ticker);

                return cached;
            }

            var oAsset = await _repository.GetByTickerAsync(ticker, cancellationToken);

            if (oAsset is null)
                throw new InvalidOperationException($"Ativo {ticker} não encontrado.");

            var oMarket = await _marketData.GetAssetAsync(ticker, cancellationToken);

            var oMarketDto = _mapper.Map<MarketAsset, MarketAssetDto>(oMarket);

            oMarketDto.SetPvp(oMarket.PriceToNav);

            oMarketDto.SetLiquidity(oMarket.Volume);

            var oAssetAnalysisDto = BuildAnalysis(oAsset, oMarketDto);

            _cache.Set(sCacheKey, oAssetAnalysisDto, TimeSpan.FromMinutes(10));

            return oAssetAnalysisDto;
        }

        public async Task<IReadOnlyCollection<AssetAnalysisDto>> AnalyzeAsync(IEnumerable<string> tickers, CancellationToken cancellationToken = default)
        {
            var oAssetAnalysisDtoList = new List<AssetAnalysisDto>();

            foreach (var ticker in tickers.Distinct())
                oAssetAnalysisDtoList.Add(await AnalyzeAsync(ticker, cancellationToken));

            return oAssetAnalysisDtoList;
        }

        public async Task<AssetComparisonDto> CompareAsync(string asset1, string asset2, CancellationToken cancellationToken = default)
        {
            var oAssetAnalysisDto1 = await AnalyzeAsync(asset1, cancellationToken);

            var oAssetAnalysisDto2 = await AnalyzeAsync(asset2, cancellationToken);

            var oSelectedAssetAnalysisDto = oAssetAnalysisDto1.Score >= oAssetAnalysisDto2.Score ? oAssetAnalysisDto1 : oAssetAnalysisDto2;

            return new AssetComparisonDto
            {
                Asset1 = oAssetAnalysisDto1,
                Asset2 = oAssetAnalysisDto2,
                Winner = oSelectedAssetAnalysisDto.Ticker,
                Justification = BuildJustification(oAssetAnalysisDto1, oAssetAnalysisDto2)
            };
        }

        private static AssetAnalysisDto BuildAnalysis(Entities.Asset asset, MarketAssetDto market)
        {
            var dScore = CalculateScore(market);

            return new AssetAnalysisDto
            {
                Ticker = asset.Ticker,
                Name = asset.Name,
                CurrentPrice = market.Price,
                DividendYield = market.DividendYield,
                Pvp = market.Pvp,
                Liquidity = market.Liquidity,
                Score = dScore,
                Recommendation = dScore >= 80 ? "Compra" : dScore >= 60 ? "Manter" : "Evitar",
                AnalyzedAt = DateTime.UtcNow
            };
        }

        private static decimal CalculateScore(MarketAssetDto market)
        {
            var dScore = 0;

            if (market.DividendYield >= 10)
                dScore += 35;

            if (market.Pvp <= 1)
                dScore += 30;

            if (market.Liquidity >= 1_000_000)
                dScore += 20;

            if (market.Vacancy <= 5)
                dScore += 15;


            return Math.Min(dScore, 100);
        }

        private static string BuildJustification(AssetAnalysisDto assetAnalysisDto1, AssetAnalysisDto assetAnalysisDto2)
        {
            if (assetAnalysisDto1.Score > assetAnalysisDto2.Score)
                return $"{assetAnalysisDto1.Ticker} apresentou maior score ({assetAnalysisDto1.Score}) que {assetAnalysisDto2.Ticker} ({assetAnalysisDto2.Score}).";

            if (assetAnalysisDto2.Score > assetAnalysisDto1.Score)
                return $"{assetAnalysisDto2.Ticker} apresentou maior score ({assetAnalysisDto2.Score}) que {assetAnalysisDto1.Ticker} ({assetAnalysisDto1.Score}).";

            return "Os ativos possuem pontuação equivalente.";
        }

        public async Task<AssetAnalysisDto> AnalyzeAsync(string ticker)
        {
            var oMarketAsset = await _marketData.GetAssetAsync(ticker);

            return new AssetAnalysisDto
            {
                Ticker = ticker,
                CurrentPrice = oMarketAsset!.Price,
            };
        }
    }
}