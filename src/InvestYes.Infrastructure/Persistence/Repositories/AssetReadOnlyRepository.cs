using InvestYes.Domain.Entities;
using InvestYes.Domain.Interfaces.Repositories;
using InvestYes.Infrastructure.Persistence.Dapper;

namespace InvestYes.Infrastructure.Persistence.Repositories;

public class AssetReadOnlyRepository : IAssetReadOnlyRepository
{
    private readonly IDapperRepository _repository;

    public AssetReadOnlyRepository(IDapperRepository repository)
    {
        _repository = repository;
    }



    public async Task<IReadOnlyCollection<Asset>> GetAllAsync(
    CancellationToken cancellationToken)
    {
        const string sql =
       """
        SELECT
            AssetId,
            Symbol,
            Name,
            Type,
            CurrentPrice,
            DividendYield
        FROM Assets
        WHERE AssetId=@pId
        """;

        var result = await _repository.QueryAsync<Asset>(
            sql,
            cancellationToken: cancellationToken);

        return result.ToList();
    }

    public async Task<Asset?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        const string sql =
       """
        SELECT
            AssetId,
            Symbol,
            Name,
            Type,
            CurrentPrice,
            DividendYield
        FROM Assets
        WHERE AssetId=@pId
        """;

        return await _repository.QuerySingleAsync<Asset>(
            sql,
            new
            {
                pAssetId = id
            }, cancellationToken);
    }

    public async Task<Asset?> GetByTickerAsync(string ticker, CancellationToken cancellationToken)
    {
        const string sql =
        """
        SELECT
            AssetId,
            Symbol,
            Name,
            Type,
            CurrentPrice,
            DividendYield
        FROM Assets
        WHERE Ticker=@pTicker
        """;

        return await _repository.QuerySingleAsync<Asset?>(
            sql,
            new
            {
                pTicker = ticker
            }, cancellationToken);
    }
}