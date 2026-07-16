namespace BuildingBlocks.Messaging.Consumers;

public interface IRabbitMqInitializer
{
    Task InitializeAsync();
}