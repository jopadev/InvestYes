using InvestYes.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace InvestYes.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly InvestYesContext _context;

    public UnitOfWork(InvestYesContext context)
    {
        _context = context;
    }

    public async Task<int> CommitAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task RollbackAsync(
        CancellationToken cancellationToken = default)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }
}