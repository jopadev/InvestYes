using InvestYes.Application.DTOs;
using MediatR;

namespace InvestYes.Application.Queries
{
    public sealed record AnalyzeAssetQuery(
    string Ticker)
    : IRequest<AssetAnalysisDto>;

     
}
