namespace InvestYes.Domain.Events;


public sealed record AssetCreatedEvent(
    Guid Id,
    string Ticker,
    string Name,
    string Type,
    DateTime CreatedAt);


public sealed record AssetUpdatedEvent(
    Guid Id,
    string Ticker,
    string Name,
    string Type,
    DateTime UpdatedAt);

public sealed record AssetDeletedEvent(
    Guid Id,
    string Ticker,
    DateTime DeletedAt);
