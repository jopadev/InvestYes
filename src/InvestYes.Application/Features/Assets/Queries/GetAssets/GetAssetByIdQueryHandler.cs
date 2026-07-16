using AutoMapper;
using global::InvestYes.Application.DTOs;
using global::InvestYes.Application.Queries;
using InvestYes.Domain.Entities;
using InvestYes.Domain.Interfaces.Repositories;
using MediatR;

namespace InvestYes.Application.Features.Assets.Queries.GetAssets;

public sealed class GetAssetByIdQueryHandler : IRequestHandler<GetAssetByIdQuery, AssetDto>
{
    private readonly IAssetReadOnlyRepository _repository;
    private readonly IMapper _mapper;
    public GetAssetByIdQueryHandler(IAssetReadOnlyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<AssetDto> Handle(GetAssetByIdQuery request,CancellationToken cancellationToken)
    {
        
        var oAsset = await _repository.GetByIdAsync(request.id,cancellationToken);

        var  oAssetDto =_mapper.Map<Asset, AssetDto>(oAsset);

        return oAssetDto;
    }
}