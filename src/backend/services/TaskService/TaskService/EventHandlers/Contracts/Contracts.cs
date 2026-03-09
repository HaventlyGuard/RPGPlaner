
namespace TaskService.EventHandlers.Contracts;

public record TaskCompletedEvent : IIntegrationEvent
{
    public Guid Id { get; init; }
    public DateTime CreationDate { get; init; }
        
    public Guid TaskId { get; init; }
    public Guid UserId { get; init; }
    public DateTime CompletedAt { get; init; }
    public int RewardAmount { get; init; }
    public string TaskTitle { get; init; }
        
    public TaskCompletedEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }
        
    public TaskCompletedEvent(Guid taskId, Guid userId, string taskTitle, int rewardAmount)
        : this()
    {
        TaskId = taskId;
        UserId = userId;
        TaskTitle = taskTitle;
        RewardAmount = rewardAmount;
        CompletedAt = DateTime.UtcNow;
    }
}
    
public record BoosterPurchasedEvent : IIntegrationEvent
{
    public Guid Id { get; init; }
    public DateTime CreationDate { get; init; }
        
    public Guid UserId { get; init; }
    public string BoosterId { get; init; }
    public string BoosterName { get; init; }
    public int Price { get; init; }
        
    public BoosterPurchasedEvent()
    {
        Id = Guid.NewGuid();
        CreationDate = DateTime.UtcNow;
    }
}