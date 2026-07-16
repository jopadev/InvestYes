using InvestYes.Domain.Entities;

namespace InvestYes.Domain.Interfaces.Repositories;

public interface IAssetReadOnlyRepository
{
    Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    Task<Asset?> GetByTickerAsync(string ticker, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<Asset?>> GetAllAsync(CancellationToken cancellationToken);
}
