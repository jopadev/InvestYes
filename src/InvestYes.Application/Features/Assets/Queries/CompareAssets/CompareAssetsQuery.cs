using InvestYes.Application.DTOs;
using MediatR;

namespace InvestYes.Application.Features.Assets.Queries.CompareAssets
{
    public sealed record CompareAssetsQuery(
    string Asset1,
    string Asset2)
    : IRequest<AssetComparisonDto>;

     
}
