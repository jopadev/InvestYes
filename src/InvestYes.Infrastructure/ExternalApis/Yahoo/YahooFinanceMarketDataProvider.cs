using System.Net;
using System.Net.Http.Json;
using InvestYes.Domain.Services;
using InvestYes.Domain.ValueObjects;
using InvestYes.Infrastructure.ExternalApis.Yahoo.Models;
using Microsoft.Extensions.Configuration;

namespace InvestYes.Infrastructure.ExternalApis.Yahoo;

public sealed class YahooFinanceMarketDataProvider : IMarketDataProvider
{

    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public string ProviderName => "Yahoo";

    public int Priority => 3;

    public bool Supports(string ticker)
        => ticker.EndsWith("3")
        || ticker.EndsWith("4")
        || ticker.EndsWith("11");

    public YahooFinanceMarketDataProvider(
        HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;

        _httpClient.BaseAddress = new Uri(
                    configuration["MarketData:YahooFinance:BaseUrl"]!);
    }


    public async Task<MarketAsset?> GetAssetAsync(
      string ticker,
      CancellationToken cancellationToken = default)
    {
        var url = $"v8/finance/chart/{ticker}.SA";


        //var response =
        //    await _httpClient.GetFromJsonAsync<YahooResponse>(
        //        url,
        //        cancellationToken);


        var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new MarketDataUnavailableException("Yahoo Finance");
            //throw new MarketDataRateLimitException(
            //    "Yahoo Finance limitou a quantidade de requisições.");

        }

        response.EnsureSuccessStatusCode();

        var yahooResponse =
           await response.Content.ReadFromJsonAsync<YahooResponse>(
               cancellationToken: cancellationToken);

        var meta = yahooResponse?
                 .Chart?
                 .Result?
                 .FirstOrDefault()?
                 .Meta;


        if (meta == null)
            return null;


        return new MarketAsset
        {
            Symbol = meta.Symbol,

            Price = meta.RegularMarketPrice,

            UpdatedAt = DateTime.UtcNow
        };
    }


    public Task<IEnumerable<MarketAsset>> GetAssetsAsync(
        IEnumerable<string> tickers,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
