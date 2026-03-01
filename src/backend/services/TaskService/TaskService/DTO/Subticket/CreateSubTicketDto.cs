using TaskService.Models;

namespace TaskService.DTO.Subticket;

public class CreateSubTicketDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description {get ;set; }
    public TaskType? TaskType { get; set; }
}