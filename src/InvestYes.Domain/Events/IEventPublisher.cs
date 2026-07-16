using System;

namespace InvestYes.BuildingBlocks.Messaging.Abstractions;

/// <summary>
/// Abstração para publicação de eventos de integração.
/// A camada Application não conhece o broker utilizado.
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publica um evento de integração.
    /// </summary>
    /// <typeparam name="TEvent">
    /// Tipo do evento.
    /// </typeparam>
    /// <param name="event">
    /// Evento a ser publicado.
    /// </param>
    /// <param name="routingKey">
    /// Chave de roteamento RabbitMQ.
    /// </param>
    /// <param name="cancellationToken">
    /// Token de cancelamento.
    /// </param>
    Task PublishAsync<TEvent>(
        TEvent @event,
        string exchange,
        string routingKey,
        CancellationToken cancellationToken = default)
        where TEvent : class;



}