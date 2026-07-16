
//using InvestYes.Application.DTOs;
//using Microsoft.Extensions.Logging;
//using System.Net.Http.Json;

//namespace InvestYes.Domain.Services
//{
//    public sealed class MarketDataProvider : IMarketDataProvider
//    {
//        private readonly HttpClient _httpClient;
//        private readonly ILogger<MarketDataProvider> _logger;

//        public MarketDataProvider(
//            HttpClient httpClient,
//            ILogger<MarketDataProvider> logger)
//        {
//            _httpClient = httpClient;
//            _logger = logger;
//        }

//        public async Task<MarketAssetDto> GetAssetAsync(
//            string ticker,
//            CancellationToken cancellationToken = default)
//        {
//            // Exemplo: substituir pelo endpoint do provedor escolhido.
//            var response = await _httpClient.GetAsync(
//                $"assets/{ticker}",
//                cancellationToken);

//            response.EnsureSuccessStatusCode();

//            var dto = await response.Content.ReadFromJsonAsync<MarketAssetDto>(
//                cancellationToken: cancellationToken);

//            return dto ?? throw new InvalidOperationException(
//                $"Nenhum dado retornado para {ticker}.");
//        }
//    }
//}
using InvestYes.Domain.ValueObjects;
using InvestYes.Infrastructure.ExternalApis.Brapi.Models;

public interface IFiiScoreService
{
    FiiScore Calculate(BrapiFiiIndicator fii);

    string GetRecommendation(int score);

    string GetValuation(decimal priceToNav);
}
