using System.Text.Json.Nodes;

namespace TaskService.Models;

public class Ticket
{
    public Guid TicketId { get; set; }
    public string ColumnId { get; set; } = "default";
    public DateTime? StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? EndDate { get; set; } = DateTime.UtcNow;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public Priority Priority { get; set; } = Priority.Easy;
    public Category? Category { get; set; } 
    public Guid UserId { get; set; }
    public TaskType TaskType { get; set; } = TaskType.Kanban;
    public int Position { get; set; }
    
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public virtual ICollection<SubTicket> SubTickets { get; set; } = null!;


}

