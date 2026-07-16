using InvestYes.Application.DTOs;
using InvestYes.Application.Interfaces;
using InvestYes.Application.Queries;
using MediatR;

namespace InvestYes.Application.Features.Assets.Queries.AnalyzeAsset;

public sealed class AnalyzeAssetQueryHandler : IRequestHandler<AnalyzeAssetQuery, AssetAnalysisDto>
{
    private readonly IInvestmentAnalysisService _analysisService;

    public AnalyzeAssetQueryHandler(IInvestmentAnalysisService analysisService)
    {
        _analysisService = analysisService;
    }

    public async Task<AssetAnalysisDto> Handle(AnalyzeAssetQuery request,CancellationToken cancellationToken)
    {
        return await _analysisService.AnalyzeAsync(request.Ticker,cancellationToken);
    }
}