using InvestYes.Application.DTOs;
using MediatR;

namespace InvestYes.Application.Features.Assets.Commands.UpdateAsset
{
    public sealed record UpdateAssetCommand(
    Guid Id,
    string Ticker,
    string Name,
    string Type
) : IRequest<AssetDto>;


}
