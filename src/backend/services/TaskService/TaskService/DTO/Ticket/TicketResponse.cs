using TaskService.Models;

namespace TaskService.DTO.Ticket;
using TaskService.Models;
public class TicketResponse
{
    public Guid TicketId { get; set; }
    public string ColumnId { get; set; } 
    public DateTime? StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; } = DateTime.UtcNow;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Easy;
    public Category? Category { get; set; } 
    public Guid UserId { get; set; }
    public TaskType TaskType { get; set; } = TaskType.Kanban;
    public int Position { get; set; }
    public bool isComplete {get; set;} = false;
    public string Color { get; set; } = "B3B3B3";
}