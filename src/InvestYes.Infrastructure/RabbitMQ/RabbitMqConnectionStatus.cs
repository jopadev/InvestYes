namespace BuildingBlocks.Messaging.Consumers;

public sealed class RabbitMqConnectionStatus
{
    public bool Connected { get; set; }

    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string ClientName { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }
}
