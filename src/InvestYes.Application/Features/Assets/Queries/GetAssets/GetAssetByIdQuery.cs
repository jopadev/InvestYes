using InvestYes.Application.DTOs;
using MediatR;

namespace InvestYes.Application.Queries
{
    public sealed record GetAssetByIdQuery(Guid id) : IRequest<AssetDto>;
}
