
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

public sealed class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<IDbConnection> CreateConnectionAsync()
    {
        IDbConnection connection =
            new NpgsqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

        return Task.FromResult(connection);
    }
}
