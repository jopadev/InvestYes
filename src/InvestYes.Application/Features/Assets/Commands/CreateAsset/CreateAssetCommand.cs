using InvestYes.Application.DTOs;
using MediatR;

namespace InvestYes.Application.Features.Assets.Commands.CreateAsset
{
    public sealed record CreateAssetCommand(
        string Ticker,
        string Name,
        string Type
    ) : IRequest<AssetDto>;
}
