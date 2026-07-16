using InvestYes.BuildingBlocks.Messaging.Abstractions;
using InvestYes.Domain.Events;
using InvestYes.Domain.Interfaces.Repositories;
using MediatR;

namespace InvestYes.Application.Features.Assets.Commands.DeleteAsset
{
    public sealed class DeleteAssetCommandHandler
    : IRequestHandler<DeleteAssetCommand, bool>
    {
        private readonly IAssetRepository _repository;
        private readonly IEventPublisher _publisher;
        public DeleteAssetCommandHandler(IAssetRepository repository, IEventPublisher publisher)
        {
            _repository = repository;
            _publisher = publisher;
        }

        public async Task<bool> Handle(
            DeleteAssetCommand request,
            CancellationToken cancellationToken)
        {
            var asset = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (asset is null)
                return false;

            await _repository.DeleteAsync(asset, cancellationToken);

            await _publisher.PublishAsync(
   
    new AssetDeletedEvent(
        asset.AssetId,
        asset.Ticker,
        DateTime.UtcNow),
     "investment.exchange",
    "asset.deleted",
    cancellationToken);

            return true;
        }
    }


}
