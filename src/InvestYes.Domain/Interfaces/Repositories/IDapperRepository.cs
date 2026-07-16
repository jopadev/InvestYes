using Dapper;

namespace InvestYes.Domain.Interfaces.Repositories;

public interface IDapperRepository
{
    Task<T?> QuerySingleAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    Task<T?> QueryFirstAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    Task<int> ExecuteAsync(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    Task<TResult?> ExecuteScalarAsync<TResult>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    Task<SqlMapper.GridReader> QueryMultipleAsync(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);

    Task<long> CountAsync(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default);
}