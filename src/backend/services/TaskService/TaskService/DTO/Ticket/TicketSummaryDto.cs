using TaskService.Models;

namespace TaskService.DTO.Ticket;
using TaskService.Models;
public class TicketSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public Category? Category { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? Deadline { get; set; }
    public string ColumnId { get; set; } = string.Empty;
    public int SubtasksCount { get; set; }
    public int CompletedSubtasksCount { get; set; }
}