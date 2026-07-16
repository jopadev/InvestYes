using InvestYes.Application.DTOs;
using MediatR;

namespace InvestYes.Application.Features.Assets.Queries.GetAssets
{
    public sealed record GetAssetsQuery() : IRequest<IReadOnlyCollection<AssetDto>>;
}
