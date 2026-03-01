using TaskService.DTO.Subticket;
using TaskService.DTO.Tag;
using TaskService.Models;

namespace TaskService.DTO.Ticket;
using TaskService.Models;
public class TicketDto
{
    public Guid Id { get; set; }
    public string ColumnId { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public int ColumnPosition { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Priority Priority { get; set; }
    public Category? Category { get; set; }
    public Guid UserId { get; set; }
    public TaskType TaskType { get; set; }
    public int Position { get; set; }
    public bool IsCompleted { get; set; }
    public string Color { get; set; } = "B3B3B3";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? Deadline { get; set; }
    
    public List<TagDto> Tags { get; set; } = new();
    public List<SubTicketDto> Subtasks { get; set; } = new();
}