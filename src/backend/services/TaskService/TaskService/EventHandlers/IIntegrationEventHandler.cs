using TaskService.EventHandlers.Contracts;

namespace TaskService.EventHandlers;

public interface IIntegrationEventHandler<in T> where T : IIntegrationEvent
{
    Task Handle(T @event);
}