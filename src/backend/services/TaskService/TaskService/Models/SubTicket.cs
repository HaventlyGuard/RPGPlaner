namespace TaskService.Models;

public class SubTicket
{
    public Guid SubTicketId { get; set; }
    public Guid TicketId { get; set; } 
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public TaskType TaskType { get; set; } = TaskType.Kanban;
    public bool isComplete {get; set;} = false;

}