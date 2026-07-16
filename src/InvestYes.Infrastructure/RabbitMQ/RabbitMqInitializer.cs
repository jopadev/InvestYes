using BuildingBlocks.Messaging.Consumers;
using RabbitMQ.Client;

namespace InvestYes.Infrastructure.RabbitMQ;

public sealed class RabbitMqInitializer : IRabbitMqInitializer
{
    private readonly IRabbitMqConnection _connection;

    public RabbitMqInitializer(IRabbitMqConnection connection)
    {
        _connection = connection;
    }

    public async Task InitializeAsync()
    {
        var channel = await _connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: "investment.exchange",
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false);

        await channel.QueueDeclareAsync(
            queue: "asset.created",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: "asset.created",
            exchange: "investment.exchange",
            routingKey: "asset.created");

        await channel.QueueDeclareAsync(
            queue: "asset.updated",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: "asset.updated",
            exchange: "investment.exchange",
            routingKey: "asset.updated");

        await channel.QueueDeclareAsync(
            queue: "asset.deleted",
            durable: true,
            exclusive: false,
            autoDelete: false);

        await channel.QueueBindAsync(
            queue: "asset.deleted",
            exchange: "investment.exchange",
            routingKey: "asset.deleted");

        await channel.CloseAsync();
    }
}