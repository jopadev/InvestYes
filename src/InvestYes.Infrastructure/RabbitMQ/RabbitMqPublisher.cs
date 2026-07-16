using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using InvestYes.BuildingBlocks.Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BuildingBlocks.Messaging.Consumers;

namespace InvestYes.Infrastructure.RabbitMQ;

public sealed class RabbitMqPublisher : IEventPublisher
{
    private readonly IRabbitMqConnection _connection;
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(
        IRabbitMqConnection connection,
        IOptions<RabbitMqOptions> options,
        ILogger<RabbitMqPublisher> logger)
    {
        _connection = connection;
        _options = options.Value;
        _logger = logger;
    }

    //public async Task PublishAsync<TEvent>(
    //    TEvent @event,
    //    string routingKey,
    //    CancellationToken cancellationToken = default)
    //    where TEvent : class
    //{
    //    ArgumentNullException.ThrowIfNull(@event);

    //    await using var channel = await _connection.CreateChannelAsync(cancellationToken);

    //    // Garante que a exchange exista
    //    await channel.ExchangeDeclareAsync(
    //        exchange: _options.Exchange,
    //        type: ExchangeType.Topic,
    //        durable: true,
    //        autoDelete: false,
    //        arguments: null,
    //        cancellationToken: cancellationToken);

    //    var properties = new BasicProperties
    //    {
    //        Persistent = true,
    //        ContentType = "application/json",
    //        ContentEncoding = "utf-8",
    //        MessageId = Guid.NewGuid().ToString(),
    //        Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
    //        Type = typeof(TEvent).Name
    //    };

    //    var json = JsonSerializer.Serialize(@event);

    //    var body = Encoding.UTF8.GetBytes(json);

    //    await channel.BasicPublishAsync(
    //        exchange: _options.Exchange,
    //        routingKey: routingKey,
    //        mandatory: false,
    //        basicProperties: properties,
    //        body: body,
    //        cancellationToken: cancellationToken);

    //    _logger.LogInformation(
    //        "Evento {Event} publicado na exchange {Exchange} utilizando routing key {RoutingKey}.",
    //        typeof(TEvent).Name,
    //        _options.Exchange,
    //        routingKey);
    //}

    public async Task PublishAsync<TEvent>(
        TEvent @event,
        string exchange,
        string routingKey,
        CancellationToken cancellationToken = default)
        where TEvent : class
    {
        await using var channel =
            await _connection.CreateChannelAsync(cancellationToken);

        await channel.ExchangeDeclareAsync(
            exchange,
            ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            cancellationToken: cancellationToken);

        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        var body = Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(@event));

        await channel.BasicPublishAsync(
            exchange,
            routingKey,
            false,
            properties,
            body,
            cancellationToken);
    }
}
