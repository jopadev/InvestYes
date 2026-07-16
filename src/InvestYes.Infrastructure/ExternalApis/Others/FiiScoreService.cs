
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

public sealed class FiiScoreService : IFiiScoreService
{
    public FiiScore Calculate(BrapiFiiIndicator fii)
    {
        decimal score = 0;

        List<string> reasons = [];

        //------------------------------------------------
        // P/VP (30 pontos)
        //------------------------------------------------

        if (fii.PriceToNav <= 0.90m)
        {
            score += 30;
            reasons.Add("P/VP abaixo de 0,90.");
        }
        else if (fii.PriceToNav <= 1.00m)
        {
            score += 25;
            reasons.Add("P/VP próximo do valor patrimonial.");
        }
        else if (fii.PriceToNav <= 1.10m)
        {
            score += 18;
        }
        else if (fii.PriceToNav <= 1.20m)
        {
            score += 10;
        }

        //------------------------------------------------
        // DY (25)
        //------------------------------------------------

        if (fii.DividendYield12m >= 0.12m)
        {
            score += 25;
            reasons.Add("Dividend Yield acima de 12%.");
        }
        else if (fii.DividendYield12m >= 0.10m)
        {
            score += 20;
        }
        else if (fii.DividendYield12m >= 0.08m)
        {
            score += 15;
        }
        else if (fii.DividendYield12m >= 0.06m)
        {
            score += 10;
        }

        //------------------------------------------------
        // Patrimônio (10)
        //------------------------------------------------

        if (fii.Equity >= 2_000_000_000)
        {
            score += 10;
            reasons.Add("Grande patrimônio líquido.");
        }
        else if (fii.Equity >= 1_000_000_000)
        {
            score += 7;
        }
        else if (fii.Equity >= 500_000_000)
        {
            score += 5;
        }

        //------------------------------------------------
        // Ativos (10)
        //------------------------------------------------

        if (fii.TotalAssets >= 2_000_000_000)
        {
            score += 10;
        }
        else if (fii.TotalAssets >= 1_000_000_000)
        {
            score += 7;
        }

        //------------------------------------------------
        // Cotistas (10)
        //------------------------------------------------

        if (fii.TotalInvestors >= 300000)
        {
            score += 10;
        }
        else if (fii.TotalInvestors >= 100000)
        {
            score += 7;
        }
        else if (fii.TotalInvestors >= 50000)
        {
            score += 5;
        }

        //------------------------------------------------
        // Retorno mensal (15)
        //------------------------------------------------

        if (fii.MonthlyReturn >= 0.02m)
        {
            score += 15;
        }
        else if (fii.MonthlyReturn >= 0.01m)
        {
            score += 10;
        }
        else if (fii.MonthlyReturn > 0)
        {
            score += 5;
        }

        var classification = score switch
        {
            >= 85 => "Excelente",

            >= 70 => "Barato",

            >= 55 => "Neutro",

            >= 40 => "Caro",

            _ => "Muito Caro"
        };

        return new FiiScore
        {
            Symbol = fii.Symbol,
            Score = score,
            Classification = classification,
            Reasons = reasons
        };
    }

    public string GetRecommendation(int score)
    {
        return score switch
        {
            >= 90 => "Compra Forte",
            >= 75 => "Comprar",
            >= 60 => "Neutro",
            >= 40 => "Cautela",
            _ => "Evitar"
        };
    }

    public string GetValuation(decimal priceToNav)
    {
        return priceToNav switch
        {
            <= 0.95m => "Barato",
            <= 1.05m => "Neutro",
            _ => "Caro"
        };
    }
}