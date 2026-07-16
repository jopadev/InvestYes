using System.Data;
using Dapper;
using InvestYes.Domain.Interfaces.Repositories;

namespace InvestYes.Infrastructure.Persistence.Dapper;

public sealed class DapperRepository : IDapperRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DapperRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    private async Task<IDbConnection> GetConnectionAsync()
    {
        return await _connectionFactory.CreateConnectionAsync();
    }

    #region Query

    public async Task<T?> QuerySingleAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await GetConnectionAsync();

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<T>(command);
    }

    public async Task<T?> QueryFirstAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await GetConnectionAsync();

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<T>(command);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await GetConnectionAsync();

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        return await connection.QueryAsync<T>(command);
    }

    #endregion

    #region Execute

    public async Task<int> ExecuteAsync(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await GetConnectionAsync();

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        return await connection.ExecuteAsync(command);
    }

    public async Task<TResult?> ExecuteScalarAsync<TResult>(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        using var connection = await GetConnectionAsync();

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        return await connection.ExecuteScalarAsync<TResult>(command);
    }

    #endregion

    #region Multiple

    public async Task<SqlMapper.GridReader> QueryMultipleAsync(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionAsync();

        var command = new CommandDefinition(
            sql,
            parameters,
            cancellationToken: cancellationToken);

        return await connection.QueryMultipleAsync(command);
    }

    #endregion

    #region Helpers

    public async Task<bool> ExistsAsync(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var result = await ExecuteScalarAsync<bool>(
            sql,
            parameters,
            cancellationToken);

        return result;
    }

    public async Task<long> CountAsync(
        string sql,
        object? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var result = await ExecuteScalarAsync<long>(
            sql,
            parameters,
            cancellationToken);

        return result;
    }

    #endregion
}