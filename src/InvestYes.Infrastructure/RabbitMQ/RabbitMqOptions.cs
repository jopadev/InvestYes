using System.ComponentModel.DataAnnotations;

namespace InvestYes.Infrastructure.RabbitMQ;


/// <summary>
/// Configurações do RabbitMQ.
/// Bind automático do appsettings.json.
/// </summary>
public sealed class RabbitMqOptions
{
    public const string SectionName = "RabbitMQ";

    /// <summary>
    /// Nome do Host.
    /// </summary>
    [Required]
    public string HostName { get; init; } = "localhost";

    /// <summary>
    /// Porta TCP.
    /// </summary>
    [Range(1, 65535)]
    public int Port { get; init; } = 5672;

    /// <summary>
    /// Virtual Host.
    /// </summary>
    public string VirtualHost { get; init; } = "/";

    /// <summary>
    /// Usuário.
    /// </summary>
    [Required]
    public string UserName { get; init; } = "guest";

    /// <summary>
    /// Senha.
    /// </summary>
    [Required]
    public string Password { get; init; } = "guest";

    /// <summary>
    /// Nome da aplicação.
    /// </summary>
    public string ClientProvidedName { get; init; } = "InvestmentPortfolio";

    /// <summary>
    /// Recovery automático da conexão.
    /// </summary>
    public bool AutomaticRecoveryEnabled { get; init; } = true;

    /// <summary>
    /// Intervalo entre tentativas de reconexão.
    /// </summary>
    public TimeSpan NetworkRecoveryInterval { get; init; }
        = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Heartbeat.
    /// </summary>
    public TimeSpan RequestedHeartbeat { get; init; }
        = TimeSpan.FromSeconds(30);

    /// <summary>
    /// SSL.
    /// </summary>
    public bool UseSsl { get; init; }

    /// <summary>
    /// Nome do servidor SSL.
    /// </summary>
    public string? SslServerName { get; init; }

    /// <summary>
    /// Exchange padrão.
    /// </summary>
    public string ExchangeName { get; init; }
        = "investment.exchange";

    /// <summary>
    /// Tipo da Exchange.
    /// </summary>
    public string ExchangeType { get; init; } = "topic";

    /// <summary>
    /// Exchange durável.
    /// </summary>
    public bool DurableExchange { get; init; } = true;

    /// <summary>
    /// Prefetch dos consumidores.
    /// </summary>
    public ushort PrefetchCount { get; init; } = 20;

    /// <summary>
    /// Número máximo de tentativas de publicação.
    /// </summary>
    public int PublishRetryCount { get; init; } = 5;

    /// <summary>
    /// Timeout da publicação.
    /// </summary>
    public TimeSpan PublishTimeout { get; init; }
        = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Tempo de expiração das mensagens.
    /// </summary>
    public TimeSpan MessageExpiration { get; init; }
        = TimeSpan.FromDays(7);

    /// <summary>
    /// Ativa Publisher Confirms.
    /// </summary>
    public bool PublisherConfirms { get; init; } = true;

    /// <summary>
    /// Ativa Publisher Returns.
    /// </summary>
    public bool PublisherReturns { get; init; } = true;

    /// <summary>
    /// DLX.
    /// </summary>
    public string DeadLetterExchange { get; init; }
        = "investment.dlx";

    /// <summary>
    /// DLQ.
    /// </summary>
    public string DeadLetterQueue { get; init; }
        = "investment.dlq";

    /// <summary>
    /// Exchange do Outbox.
    /// </summary>
    public string OutboxExchange { get; init; }
        = "investment.outbox";

    /// <summary>
    /// Nome da fila do Outbox.
    /// </summary>
    public string OutboxQueue { get; init; }
        = "investment.outbox.queue";
}

