using InvestYes.Application.DTOs;

namespace InvestYes.Application.Interfaces
{
    public interface IInvestmentAnalysisService
    {
        Task<AssetAnalysisDto> AnalyzeAsync(
            string ticker,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<AssetAnalysisDto>> AnalyzeAsync(
            IEnumerable<string> tickers,
            CancellationToken cancellationToken = default);

        Task<AssetComparisonDto> CompareAsync(
            string asset1,
            string asset2,
            CancellationToken cancellationToken = default);
    }
}
