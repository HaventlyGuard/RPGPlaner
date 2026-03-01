using TaskService.Models;

namespace TaskService.DTO.Subticket;

public class SubTicketDto
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskType TaskType { get; set; }
    public bool IsCompleted { get; set; }
}