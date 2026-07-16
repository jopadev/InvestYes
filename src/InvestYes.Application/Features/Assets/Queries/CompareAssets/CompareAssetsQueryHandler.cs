using InvestYes.Application.DTOs;
using InvestYes.Application.Interfaces;
using MediatR;

namespace InvestYes.Application.Features.Assets.Queries.CompareAssets;

public sealed class CompareAssetsQueryHandler : IRequestHandler<CompareAssetsQuery, AssetComparisonDto>
{
    private readonly IInvestmentAnalysisService _analysisService;

    public CompareAssetsQueryHandler(IInvestmentAnalysisService analysisService)
    {
        _analysisService = analysisService;
    }

    public async Task<AssetComparisonDto> Handle(CompareAssetsQuery request,CancellationToken cancellationToken)
    {
        return await _analysisService.CompareAsync(request.Asset1,request.Asset2,cancellationToken);
    }
}