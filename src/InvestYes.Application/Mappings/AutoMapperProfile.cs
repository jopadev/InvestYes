using AutoMapper;
using InvestYes.Application.DTOs;
using InvestYes.Domain.Entities;
using InvestYes.Domain.ValueObjects;

namespace InvestYes.Application.Mappings;

public sealed class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Asset, AssetDto>();

        CreateMap<AssetDto, Asset>();

        CreateMap<MarketAsset, MarketAssetDto>().ReverseMap();

        CreateMap<UserDto, User>().ReverseMap();
    }
}