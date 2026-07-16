using BuildingBlocks.Messaging.Consumers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics;

namespace InvestYes.Infrastructure.RabbitMQ;

/// <summary>
/// Gerencia uma única conexão com RabbitMQ.
/// Compatível com RabbitMQ.Client 7.x.
/// </summary>
public sealed partial class RabbitMqConnection : IRabbitMqConnection
{
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqConnection> _logger;

    /// <summary>
    /// Factory utilizada para criação da conexão.
    /// </summary>
    private readonly ConnectionFactory _factory;

    /// <summary>
    /// Conexão compartilhada.
    /// </summary>
    private IConnection? _connection;

    /// <summary>
    /// Sincroniza tentativas simultâneas de conexão.
    /// </summary>
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    /// <summary>
    /// Indica descarte do objeto.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// ActivitySource para OpenTelemetry.
    /// </summary>
    private static readonly ActivitySource Activity = new("BuildingBlocks.Messaging.RabbitMQ");

    /// <summary>
    /// Indica se existe conexão ativa.
    /// </summary>
    public bool IsConnected =>
        _connection is
        {
            IsOpen: true
        } &&
        !_disposed;

    /// <summary>
    /// Cria uma instância do gerenciador de conexão.
    /// </summary>
    public RabbitMqConnection(IOptions<RabbitMqOptions> options,ILogger<RabbitMqConnection> logger)
    {
        _options = options.Value;
        _logger = logger;

        _factory = BuildFactory();

        _logger.LogInformation(
            "RabbitMQ Connection Manager criado. Host={Host} Porta={Port}",
            _options.HostName,
            _options.Port);
    }

    /// Factory
    /// <summary>
    /// Cria a ConnectionFactory.
    /// </summary>
    private ConnectionFactory BuildFactory()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.HostName,

            Port = _options.Port,

            UserName = _options.UserName,

            Password = _options.Password,

            VirtualHost = _options.VirtualHost,

            RequestedHeartbeat =
                _options.RequestedHeartbeat,

            AutomaticRecoveryEnabled =
                _options.AutomaticRecoveryEnabled,

            NetworkRecoveryInterval =
                _options.NetworkRecoveryInterval,

            ClientProvidedName =
                _options.ClientProvidedName,

        };

        if (_options.UseSsl)
        {
            factory.Ssl.Enabled = true;

            factory.Ssl.ServerName =
                _options.SslServerName ??
                _options.HostName;
        }

        return factory;
    }

    //Inicialização
    /// <summary>
    /// Inicializa a conexão.
    /// </summary>
    public async Task<bool> InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (IsConnected)
        {
            return true;
        }

        return await TryConnectAsync(cancellationToken);
    }

    /// <summary>
    /// Tenta estabelecer uma conexão com RabbitMQ.
    /// 
    /// Utiliza SemaphoreSlim para impedir múltiplas
    /// threads tentando criar conexões simultâneas.
    /// </summary>
    public async Task<bool> TryConnectAsync(CancellationToken cancellationToken = default)
    {
        if (IsConnected)
            return true;


        await _connectionLock.WaitAsync(
            cancellationToken);

        try
        {
            if (IsConnected)
                return true;


            using var activity =
                Activity.StartActivity(
                    "rabbitmq.connect");


            _logger.LogInformation(
                "Tentando conectar ao RabbitMQ. Host={Host}:{Port}",
                _options.HostName,
                _options.Port);


            try
            {
                var connection =
                    await _factory.CreateConnectionAsync(
                        cancellationToken);


                RegisterConnectionEvents(connection);


                _connection = connection;


                _logger.LogInformation(
                    "RabbitMQ conectado com sucesso. Client={Client}",
                    _options.ClientProvidedName);


                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Falha ao conectar no RabbitMQ");


                return false;
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    /// <summary>
    /// Registra eventos do ciclo de vida da conexão.
    /// </summary>
    private void RegisterConnectionEvents(IConnection connection)
    {

        connection.ConnectionShutdownAsync +=
            async (_, args) =>
            {
                _logger.LogWarning(
                    "RabbitMQ ConnectionShutdown. Reason={Reason}",
                    args.ReplyText);


                await Task.CompletedTask;
            };


        connection.ConnectionBlockedAsync +=
            async (_, args) =>
            {
                _logger.LogWarning(
                    "RabbitMQ ConnectionBlocked. Reason={Reason}",
                    args.Reason);


                await Task.CompletedTask;
            };


        connection.ConnectionUnblockedAsync +=
            async (_, _) =>
            {
                _logger.LogInformation(
                    "RabbitMQ ConnectionUnblocked");


                await Task.CompletedTask;
            };


        connection.CallbackExceptionAsync +=
            async (_, args) =>
            {
                _logger.LogError(
                    args.Exception,
                    "RabbitMQ CallbackException");


                await Task.CompletedTask;
            };
    }

    /// <summary>
    /// Garante que exista uma conexão disponível.
    /// </summary>
    private async Task EnsureConnectedAsync(
        CancellationToken cancellationToken)
    {
        if (IsConnected)
            return;


        var connected =
            await TryConnectAsync(
                cancellationToken);


        if (!connected)
        {
            throw new InvalidOperationException(
                "Não foi possível estabelecer conexão com RabbitMQ.");
        }
    }

    /// <summary>
    /// Valida se a conexão RabbitMQ está disponível.
    /// </summary>
    public Task<bool> CheckHealthAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(IsConnected);
    }

    /// <summary>
    /// Cria um novo canal RabbitMQ.
    ///
    /// A conexão é compartilhada,
    /// porém cada operação recebe um canal próprio.
    /// </summary>
    public async Task<IChannel> CreateChannelAsync(CancellationToken cancellationToken = default)
    {
        await EnsureConnectedAsync(cancellationToken);

        try
        {
            using var activity =
                Activity.StartActivity(
                    "rabbitmq.create_channel");


            var channelOptions = new CreateChannelOptions(
    publisherConfirmationsEnabled: _options.PublisherConfirms,
    publisherConfirmationTrackingEnabled: _options.PublisherConfirms);

            var channel =
                await _connection!
                    .CreateChannelAsync(
                        channelOptions,
                        cancellationToken);


            await ConfigureChannelAsync(
                channel,
                cancellationToken);


            _logger.LogDebug(
                "Canal RabbitMQ criado. ChannelNumber={Channel}",
                channel.ChannelNumber);


            return channel;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro criando canal RabbitMQ");

            throw;
        }
    }

    /// <summary>
    /// Configura propriedades padrão do canal.
    /// </summary>
    private async Task ConfigureChannelAsync(
        IChannel channel,
        CancellationToken cancellationToken)
    {


        /*
         * QoS
         *
         * Controla quantas mensagens
         * um consumer recebe simultaneamente.
         */
        await channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount:
                _options.PrefetchCount,
            global: false,
            cancellationToken);


        _logger.LogDebug(
            "QoS configurado. Prefetch={Prefetch}",
            _options.PrefetchCount);
    }

    /// <summary>
    /// Verifica se um canal ainda está disponível.
    /// </summary>
    private static bool IsChannelOpen(
        IChannel? channel)
    {
        return channel is not null
            && channel.IsOpen;
    }

    /// <summary>
    /// Cria canal garantindo conexão ativa.
    /// </summary>
    public async Task<IChannel> GetOrCreateChannelAsync(CancellationToken cancellationToken = default)
    {
        if (!IsConnected)
            await EnsureConnectedAsync(cancellationToken);

        return await CreateChannelAsync(cancellationToken);
    }

    /// <summary>
    /// Executado quando o RabbitMQ encerra a conexão.
    /// </summary>
    private async Task OnConnectionShutdownAsync(object sender, ShutdownEventArgs args)
    {
        _logger.LogWarning(
            "RabbitMQ Connection Shutdown. " +
            "Code={Code}, Text={Text}, Initiator={Initiator}",
            args.ReplyCode,
            args.ReplyText,
            args.Initiator);


        await Task.CompletedTask;
    }


    private async Task OnConnectionUnblockedAsync(object sender,AsyncEventArgs args)
    {
        _logger.LogInformation("RabbitMQ Connection Unblocked");

        await Task.CompletedTask;
    }

    /// <summary>
    /// Verifica disponibilidade do RabbitMQ.
    /// </summary>
    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!IsConnected)
                return false;

            await using var channel = await _connection!.CreateChannelAsync(null,cancellationToken);

            return channel.IsOpen;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Falha no Health Check RabbitMQ");

            return false;
        }
    }

    public RabbitMqConnectionStatus GetStatus()
    {
        return new RabbitMqConnectionStatus
        {
            Connected = IsConnected,
            Host = _options.HostName,
            Port = _options.Port,
            ClientName = _options.ClientProvidedName,
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Libera recursos RabbitMQ.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        try
        {
            if (_connection is not null)
            {
                if (_connection.IsOpen)
                    await _connection.CloseAsync();

                await _connection.DisposeAsync();
            }

            _logger.LogInformation("RabbitMQ Connection encerrada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,"Erro ao fechar conexão RabbitMQ");
        }
        finally
        {
            _connectionLock.Dispose();
        }
    }
}
