using Microsoft.EntityFrameworkCore.Metadata;
using RabbitMQ.Client;


namespace TaskService.EventHandlers.Contracts;

public record TaskCompleteEvent : IIntegrationEvent
{
    public Guid Id { get; }
    public DateTime CreationDate { get; }
    
    public Guid TaskId { get; init; }
    public Guid UserId { get; init; }
    public DateTime CompletedAt { get; init; }
    public int RewardAmount { get; init; }
    public string TaskTitle { get; init; }
        
    public TaskCompleteEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }
        
    public TaskCompleteEvent(Guid taskId, Guid userId, string taskTitle, int rewardAmount)
        : this()
    {
        TaskId = taskId;
        UserId = userId;
        TaskTitle = taskTitle;
        RewardAmount = rewardAmount;
        CompletedAt = DateTime.UtcNow;
    }
}