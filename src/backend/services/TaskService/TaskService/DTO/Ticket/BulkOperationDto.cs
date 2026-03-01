using TaskService.Models;

namespace TaskService.DTO.Ticket;

public class BulkOperationDto
{
    public List<Guid> TaskIds { get; set; } = new();
    public string Operation { get; set; } = string.Empty; // delete, complete, move, changepriority
    public string? TargetColumnId { get; set; }
    public Priority? Priority { get; set; }
}