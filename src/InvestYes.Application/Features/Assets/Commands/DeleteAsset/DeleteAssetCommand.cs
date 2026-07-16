using MediatR;

namespace InvestYes.Application.Features.Assets.Commands.DeleteAsset
{
    public sealed record DeleteAssetCommand(Guid Id) : IRequest<bool>;


}
