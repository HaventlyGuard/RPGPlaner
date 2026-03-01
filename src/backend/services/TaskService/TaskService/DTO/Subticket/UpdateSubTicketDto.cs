using TaskService.Models;

namespace TaskService.DTO.Subticket;

public class UpdateSubTicketDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public TaskType? TaskType { get; set; }
    public bool? IsCompleted { get; set; }
}