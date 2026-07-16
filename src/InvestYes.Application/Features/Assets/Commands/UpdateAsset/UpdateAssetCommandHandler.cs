using AutoMapper;
using InvestYes.Application.DTOs;
using InvestYes.BuildingBlocks.Messaging.Abstractions;
using InvestYes.Domain.Enums;
using InvestYes.Domain.Events;
using InvestYes.Domain.Interfaces.Repositories;
using MediatR;

namespace InvestYes.Application.Features.Assets.Commands.UpdateAsset
{
    public sealed class UpdateAssetCommandHandler
    : IRequestHandler<UpdateAssetCommand, AssetDto>
    {
        private readonly IAssetRepository _repository;
        private readonly IMapper _mapper;
        private readonly IEventPublisher _publisher;
        public UpdateAssetCommandHandler(
            IAssetRepository repository,
            IMapper mapper,
            IEventPublisher publisher)
        {
            _repository = repository;
            _mapper = mapper;
            _publisher = publisher;
        }

        public async Task<AssetDto> Handle(
            UpdateAssetCommand request,
            CancellationToken cancellationToken)
        {
            var asset = await _repository.GetByIdAsync(request.Id, cancellationToken);

            if (asset is null)
                throw new Exception("Ativo não encontrado.");

            if (!Enum.TryParse<AssetType>(request.Type, true, out var assetType))
                throw new ArgumentException("Tipo de ativo inválido.");

            asset.Name = request.Name;
            asset.Ticker = request.Ticker.ToUpper();
            asset.Type = assetType;

            await _repository.UpdateAsync(asset, cancellationToken);

            await _publisher.PublishAsync(

    new AssetUpdatedEvent(
        asset.AssetId,
        asset.Ticker,
        asset.Name,
        asset.Type.ToString(),
        DateTime.UtcNow),
     "investment.exchange",
    "asset.updated",
    cancellationToken);

            return _mapper.Map<AssetDto>(asset);
        }
    }


}
