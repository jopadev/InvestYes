using RabbitMQ.Client;

namespace BuildingBlocks.Messaging.Consumers;

/// <summary>
/// Gerencia uma conexão única com o RabbitMQ e fornece canais sob demanda.
/// Thread-safe.
/// </summary>
public interface IRabbitMqConnection : IAsyncDisposable
{
    /// <summary>
    /// Indica se a conexão está aberta.
    /// </summary>
    bool IsConnected { get; }

    Task<bool> InitializeAsync(
    CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo canal para publicação ou consumo.
    /// Cada operação deve utilizar seu próprio canal.
    /// </summary>
    Task<IChannel> CreateChannelAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Garante que exista uma conexão ativa.
    /// Caso não exista, tenta reconectar.
    /// </summary>
    Task<bool> TryConnectAsync(
        CancellationToken cancellationToken = default);

    Task<bool> HealthCheckAsync(
        CancellationToken cancellationToken = default);
}
