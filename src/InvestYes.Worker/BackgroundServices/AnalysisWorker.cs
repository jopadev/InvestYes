using BuildingBlocks.Messaging.Consumers;
using InvestYes.Domain.Events;
using Microsoft.Extensions.Hosting;

namespace InvestYes.Worker.BackgroundServices;

public sealed class AnalysisWorker : BackgroundService
{
    private readonly RabbitMqConsumer<AssetCreatedEvent> _consumer;

    public AnalysisWorker(RabbitMqConsumer<AssetCreatedEvent> consumer)
    {
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //await _consumer.StartConsumerAsync(
        //    async message =>
        //    {
        //        // analisar investimento
        //    },
        //    stoppingToken);

        await _consumer.StartConsumerAsync(stoppingToken);
    }
}

