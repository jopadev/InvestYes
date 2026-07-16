using System.Net.Http.Headers;
using System.Net.Http.Json;
using InvestYes.Domain.Services;
using InvestYes.Domain.ValueObjects;
using InvestYes.Infrastructure.ExternalApis.Brapi.Models;
using Microsoft.Extensions.Configuration;

namespace InvestYes.Infrastructure.ExternalServices.Brapi;

using System.Text.Json;

public static class JsonHelper
{
    public static bool TryParseJson(
        string json,
        out JsonDocument? document)
    {
        document = null;

        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            document = JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}

public sealed class BrapiMarketDataProvider : IMarketDataProvider
{

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public string ProviderName => "BRAPI";
    private readonly IFiiScoreService _fiiScoreService;

    public int Priority => 2;

    public bool Supports(string ticker)
        => ticker.EndsWith("3")
        || ticker.EndsWith("4")
        || ticker.EndsWith("11");

    public BrapiMarketDataProvider(HttpClient httpClient, IConfiguration configuration, IFiiScoreService fiiScoreService)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(configuration["MarketData:Brapi:BaseUrl"]);
        _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + configuration["MarketData:Brapi:Token"]);
        _fiiScoreService = fiiScoreService;
    }

    public async Task<MarketAsset?> GetAssetAsync(string ticker, CancellationToken cancellationToken = default)
    {
        var response =
            await _httpClient.GetFromJsonAsync<BrapiResponse>(
                $"quote/{ticker}",
                cancellationToken);


        var quote =
            response?
            .Results?
            .FirstOrDefault();


        if (quote == null)
            return null;


        return new MarketAsset
        {
            Symbol = quote.Symbol,

            Name = quote.LongName,

            Price = quote.RegularMarketPrice,

            ChangePercent =
                quote.RegularMarketChangePercent,

            UpdatedAt =
                DateTime.UtcNow
        };
    }

    public async Task<MarketAsset?> GetAsset3Async(string ticker, CancellationToken cancellationToken = default)
    {
        var result =
            await _httpClient.GetStringAsync(
                 $"v2/fii/indicators?symbols={ticker}",
                cancellationToken);

        var response = JsonSerializer.Deserialize<BrapiFiiIndicatorsResponse>(
        result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        var fii = response?.Fiis.FirstOrDefault();

        if (fii is null)
            return null;

        var oScore = _fiiScoreService.Calculate(fii);

        return new MarketAsset
        {
            Symbol = fii.Symbol,
            Name = fii.Name,
            Price = fii.Price,
            DividendYield = fii.DividendYield12m,
            MarketCap = fii.NavPerShare,
            ChangePercent = fii.PriceToNav,

            //PriceToBook = fii.PriceToNav,
            //Equity = fii.Equity,
            //TotalAssets = fii.TotalAssets,
            //Segment = fii.SegmentoAtuacao,
            UpdatedAt = DateTime.UtcNow
        };

    }

    public async Task<IEnumerable<MarketAsset>> GetAssetsAsync(
        IEnumerable<string> tickers,
        CancellationToken cancellationToken = default)
    {

        var result = new List<MarketAsset>();

        foreach (var ticker in tickers)
        {
            var asset =
                await GetAssetAsync(
                    ticker,
                    cancellationToken);

            if (asset != null)
                result.Add(asset);
        }


        return result;
    }

    public static bool TryDeserialize<T>(
    string json,
    out T? result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            result = JsonSerializer.Deserialize<T>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return result != null;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
