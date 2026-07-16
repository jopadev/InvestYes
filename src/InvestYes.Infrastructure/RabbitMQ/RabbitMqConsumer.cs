using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using InvestYes.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BuildingBlocks.Messaging.Consumers;

/// <summary>
/// Classe base para consumidores RabbitMQ.
/// Compatível com RabbitMQ.Client 7.x
/// </summary>
public abstract class RabbitMqConsumer<TMessage> : BackgroundService, IAsyncDisposable
    where TMessage : class
{
    private readonly IRabbitMqConnection _connection;
    private readonly RabbitMqOptions _options;
    private readonly ILogger _logger;

    private IChannel? _channel;
    private AsyncEventingBasicConsumer? _consumer;

    private readonly ActivitySource _activitySource;

    protected RabbitMqConsumer(
        IRabbitMqConnection connection,
        IOptions<RabbitMqOptions> options,
        ILogger logger)
    {
        _connection = connection;
        _options = options.Value;
        _logger = logger;

        _activitySource = new ActivitySource(GetType().Name);
    }

    /// <summary>
    /// Nome da fila.
    /// </summary>
    protected abstract string QueueName { get; }

    /// <summary>
    /// Routing Key.
    /// </summary>
    protected abstract string RoutingKey { get; }

    /// <summary>
    /// Exchange.
    /// </summary>
    protected virtual string Exchange
        => _options.ExchangeName;

    /// <summary>
    /// Tipo da Exchange.
    /// </summary>
    protected virtual string ExchangeType
        => _options.ExchangeType;

    /// <summary>
    /// AutoAck sempre falso.
    /// </summary>
    protected virtual bool AutoAck => false;

    /// <summary>
    /// Número máximo de mensagens simultâneas.
    /// </summary>
    protected virtual ushort PrefetchCount
        => _options.PrefetchCount;

    protected override async Task ExecuteAsync(
    CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Inicializando consumer {Consumer}",
            GetType().Name);

        await CreateChannelAsync(stoppingToken);

        await StartConsumerAsync(stoppingToken);

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }

    private async Task CreateChannelAsync(
    CancellationToken cancellationToken)
    {
        _channel = await _connection.CreateChannelAsync(
            cancellationToken);

        await _channel.ExchangeDeclareAsync(
            exchange: Exchange,
            type: ExchangeType,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            cancellationToken: cancellationToken);

        await _channel.QueueBindAsync(
            QueueName,
            Exchange,
            RoutingKey,
            cancellationToken: cancellationToken);

        await _channel.BasicQosAsync(
            prefetchSize: 0,
            prefetchCount: PrefetchCount,
            global: false,
            cancellationToken: cancellationToken);

        _logger.LogInformation(
            "Fila {Queue} criada com sucesso",
            QueueName);
    }

    public async Task StartConsumerAsync(
        CancellationToken cancellationToken)
    {
        if (_channel is null)
        {
            throw new InvalidOperationException(
                "Canal RabbitMQ não inicializado.");
        }


        _consumer = new AsyncEventingBasicConsumer(
            _channel);


        _consumer.ReceivedAsync +=
            OnMessageReceivedAsync;


        var consumerTag =
            await _channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: AutoAck,
                consumer: _consumer,
                cancellationToken: cancellationToken);


        _logger.LogInformation(
            "Consumer iniciado. Queue={Queue}, ConsumerTag={ConsumerTag}",
            QueueName,
            consumerTag);
    }

    protected abstract Task HandleMessageAsync(
    TMessage message,
    CancellationToken cancellationToken);

    private async Task OnMessageReceivedAsync(
    object sender,
    BasicDeliverEventArgs args)
    {
        try
        {
            var body =
                args.Body.ToArray();


            var json =
                Encoding.UTF8.GetString(body);


            var message =
                JsonSerializer.Deserialize<TMessage>(
                    json);


            if (message is null)
            {
                throw new InvalidOperationException(
                    "Mensagem inválida.");
            }


            await HandleMessageAsync(
                message,
                CancellationToken.None);


            if (!AutoAck)
            {
                await _channel!.BasicAckAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false);
            }


            _logger.LogInformation(
                "Mensagem processada com sucesso. Queue={Queue}, DeliveryTag={Tag}",
                QueueName,
                args.DeliveryTag);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro processando mensagem. Queue={Queue}",
                QueueName);


            if (!AutoAck)
            {
                await _channel!.BasicNackAsync(
                    deliveryTag: args.DeliveryTag,
                    multiple: false,
                    requeue: false);
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_channel is not null)
            {
                if (_channel.IsOpen)
                {
                    await _channel.CloseAsync();
                }

                await _channel.DisposeAsync();
            }

            _logger.LogInformation(
                "RabbitMQ Consumer finalizado. Queue={Queue}",
                QueueName);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Erro ao liberar recursos do consumer RabbitMQ. Queue={Queue}",
                QueueName);
        }
    }

}


