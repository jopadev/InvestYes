using AutoMapper;
using global::InvestYes.Application.DTOs;
using InvestYes.Domain.Interfaces.Repositories;
using MediatR;

namespace InvestYes.Application.Features.Assets.Queries.GetAssets;

public sealed class GetAssetsQueryHandler : IRequestHandler<GetAssetsQuery, IReadOnlyCollection<AssetDto>>
{
    private readonly IAssetRepository _repository;
    private readonly IMapper _mapper;

    public GetAssetsQueryHandler(IAssetRepository repository,IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyCollection<AssetDto>> Handle(GetAssetsQuery request,CancellationToken cancellationToken)
    {
        var oAssetList = await _repository.GetAllAsync(cancellationToken);

        return _mapper.Map<IReadOnlyCollection<AssetDto>>(oAssetList);
    }
}
