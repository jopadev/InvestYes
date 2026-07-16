using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using BuildingBlocks.Messaging.Consumers;
using InvestYes.Domain.Events;

namespace InvestYes.Infrastructure.RabbitMQ.Consumers;

/// <summary>
/// Consumer responsável por processar ativos criados.
/// 
/// Exemplos:
/// - Atualizar cache Redis
/// - Criar histórico
/// - Disparar análise de mercado
/// - Alimentar dashboards
/// </summary>
public sealed class AssetCreatedConsumer
    : RabbitMqConsumer<AssetCreatedEvent>
{
    private readonly ILogger<AssetCreatedConsumer> _logger;


    public AssetCreatedConsumer(
        IRabbitMqConnection connection,
        IOptions<RabbitMqOptions> options,
        ILogger<AssetCreatedConsumer> logger)
        : base(
            connection,
            options,
            logger)
    {
        _logger = logger;
    }


    protected override string QueueName
        => "investyes.asset.created.queue";


    protected override string RoutingKey
        => "asset.created";


    protected override string Exchange
        => "investyes.events";


    protected override async Task HandleMessageAsync(
        AssetCreatedEvent message,
        CancellationToken cancellationToken)
    {

        _logger.LogInformation(
            "Processando AssetCreatedEvent. AssetId={Id}, Ticker={Ticker}",
            message.Id,
            message.Ticker);


        /*
         * 1 - Atualizar cache Redis
         *
         * await _cache.SetAsync(
         *      $"asset:{message.Ticker}",
         *      message);
         */


        /*
         * 2 - Criar histórico
         *
         * await _historyRepository.AddAsync(...)
         */


        /*
         * 3 - Solicitar análise de mercado
         *
         * await _analysisService.AnalyzeAsync(...)
         */


        await Task.CompletedTask;


        _logger.LogInformation(
            "AssetCreatedEvent processado com sucesso. Ticker={Ticker}",
            message.Ticker);
    }
}