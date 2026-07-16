using InvestYes.Domain.Entities;
using InvestYes.Domain.Interfaces.Repositories;
using InvestYes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvestYes.Infrastructure.Persistence.Repositories;

public sealed class AssetRepository : IAssetRepository
{
    private readonly InvestYesContext _context;

    public AssetRepository(InvestYesContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Asset asset,CancellationToken cancellationToken)
    {
        await _context.Assets.AddAsync(asset, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Asset asset,CancellationToken cancellationToken)
    {
        _context.Assets.Update(asset);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Asset asset,CancellationToken cancellationToken)
    {
        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id,CancellationToken cancellationToken)
    {
        var asset = await _context.Assets.FindAsync(id);

        if (asset is not null)
        {
            _context.Assets.Remove(asset);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public Task<Asset?> GetByIdAsync(Guid id,CancellationToken cancellationToken)
    {
        return _context.Assets.FirstOrDefaultAsync(x => x.AssetId == id, cancellationToken);
    }

    public Task<Asset?> GetByTickerAsync(string ticker,CancellationToken cancellationToken)
    {
        return _context.Assets.AsNoTracking().FirstOrDefaultAsync(x => x.Ticker == ticker, cancellationToken);
    }

    public async Task<IReadOnlyList<Asset>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _context.Assets
            .AsNoTracking()
            .OrderBy(x => x.Ticker)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(string ticker,CancellationToken cancellationToken)
    {
        return _context.Assets
            .AnyAsync(x => x.Ticker == ticker, cancellationToken);
    }
}

