using TaskService.Models;

namespace TaskService.DTO.Ticket;
using TaskService.Models;
public class CreateTicketDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ColumnId { get; set; } = "default";
    public Priority? Priority { get; set; }
    public Category? Category { get; set; }
    public TaskType? TaskType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Color { get; set; }
    public Guid UserId { get; set; }
}