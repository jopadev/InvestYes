using InvestYes.Domain.Services;
using InvestYes.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;

namespace InvestYes.Infrastructure.ExternalApis.FundsExplorer;

public class FundsExplorerCrawlerProvider : IMarketDataProvider
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    public string ProviderName => "FundsExplorer";

    public int Priority => 1;

    public FundsExplorerCrawlerProvider(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["MarketData:FundsExplorer:BaseUrl"]);
    }

    public async Task<MarketAsset?> GetAssetAsync(string symbol, CancellationToken cancellationToken)
    {
        try
        {
            var oHttpRequest = new HttpRequestMessage(HttpMethod.Get, $"{symbol}");

            var oHttpResponse = await _httpClient.SendAsync(oHttpRequest, cancellationToken);

            oHttpResponse.EnsureSuccessStatusCode();

            var sHtml = await oHttpResponse.Content.ReadAsStringAsync(cancellationToken);

            return FundsExplorerParser.Parse(sHtml);

        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public bool Supports(string ticker) => ticker.EndsWith("3") || ticker.EndsWith("4") || ticker.EndsWith("11");

    public Task<IEnumerable<MarketAsset>> GetAssetsAsync(IEnumerable<string> tickers, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}