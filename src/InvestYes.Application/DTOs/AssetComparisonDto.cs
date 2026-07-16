namespace InvestYes.Application.DTOs
{
    public sealed class AssetComparisonDto
    {
        public AssetAnalysisDto Asset1 { get; init; } = default!;

        public AssetAnalysisDto Asset2 { get; init; } = default!;

        public string Winner { get; init; } = string.Empty;

        public string Justification { get; init; } = string.Empty;
    }
}
