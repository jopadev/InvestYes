using InvestYes.Domain.Entities;

namespace InvestYes.Domain.Interfaces.Repositories;

public interface IAssetRepository
{
    Task AddAsync(
        Asset asset,
        CancellationToken cancellationToken);

    Task UpdateAsync(
        Asset asset,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Asset asset,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Asset?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Asset?> GetByTickerAsync(
        string ticker,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Asset>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        string ticker,
        CancellationToken cancellationToken);
}
