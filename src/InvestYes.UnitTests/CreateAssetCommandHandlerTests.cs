using AutoMapper;
using FluentAssertions;
using InvestYes.Application.DTOs;
using InvestYes.Application.Features.Assets.Commands.CreateAsset;
using InvestYes.BuildingBlocks.Messaging.Abstractions;
using InvestYes.Domain.Entities;
using InvestYes.Domain.Enums;
using InvestYes.Domain.Events;
using InvestYes.Domain.Interfaces.Repositories;
using Moq;

namespace InvestYes.UnitTests
{
    public class CreateAssetCommandHandlerTests
    {
        private readonly Mock<IAssetRepository> _repository;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly Mock<IMapper> _mapper;
        private readonly Mock<IEventPublisher> _publisher;

        private readonly CreateAssetCommandHandler _handler;

        public CreateAssetCommandHandlerTests()
        {
            _repository = new();

            _mapper = new();

            _unitOfWork = new();

            _publisher = new();

            _handler =
                new CreateAssetCommandHandler(
                    _repository.Object,
                    _mapper.Object,
                    _unitOfWork.Object,
                    _publisher.Object);
        }

        [Fact]
        public async Task Should_Create_Asset()
        {
            var command = new CreateAssetCommand(
                "MXRF11",
                "Maxi Renda",
                AssetType.FII.ToString());
            _mapper
    .Setup(x => x.Map<AssetDto>(It.IsAny<Asset>()))
    .Returns((Asset asset) => new AssetDto
    {
        AssetId = asset.AssetId,
        Ticker = asset.Ticker,
        Name = asset.Name,
        Type = asset.Type.ToString()
    });

            var dto =
                await _handler.Handle(
                    command,
                    CancellationToken.None);

            dto.Ticker.Should().Be("MXRF11");

            _repository.Verify(x =>
                x.AddAsync(
                    It.IsAny<Asset>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _unitOfWork
    .Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
    .ReturnsAsync(1);

            _publisher.Verify(x =>
                x.PublishAsync(
                    It.IsAny<AssetCreatedEvent>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
