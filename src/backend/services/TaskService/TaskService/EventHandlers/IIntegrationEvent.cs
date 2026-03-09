namespace TaskService.EventHandlers.Contracts;

public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime CreationDate { get; }
}