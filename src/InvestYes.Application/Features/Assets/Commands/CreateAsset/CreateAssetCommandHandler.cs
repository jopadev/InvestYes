using AutoMapper;
using InvestYes.Application.DTOs;
using InvestYes.BuildingBlocks.Messaging.Abstractions;
using InvestYes.Domain.Entities;
using InvestYes.Domain.Enums;
using InvestYes.Domain.Events;
using InvestYes.Domain.Interfaces.Repositories;
using InvestYes.Infrastructure.Persistence;
using MediatR;
using System.Threading.Channels;

namespace InvestYes.Application.Features.Assets.Commands.CreateAsset
{
    public sealed class CreateAssetCommandHandler
    : IRequestHandler<CreateAssetCommand, AssetDto>
    {
        private readonly IAssetRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEventPublisher _publisher;
        public CreateAssetCommandHandler(
            IAssetRepository repository,
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IEventPublisher publisher)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _publisher = publisher;
        }

        public async Task<AssetDto> Handle(
            CreateAssetCommand request,
            CancellationToken cancellationToken)
        {

            if (!Enum.TryParse<AssetType>(request.Type, true, out var assetType))
                throw new ArgumentException("Tipo de ativo inválido.");

            var asset = new Asset
            {
                AssetId = Guid.NewGuid(),
                Ticker = request.Ticker.ToUpper(),
                Name = request.Name,
                Type = assetType,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(asset, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            await _publisher.PublishAsync(new AssetCreatedEvent(
                                            asset.AssetId,
                                            asset.Ticker,
                                            asset.Name,
                                            asset.Type.ToString(),
                                            asset.CreatedAt), "investment.exchange", "asset.created", cancellationToken);

            return _mapper.Map<AssetDto>(asset);
        }
    }


}
