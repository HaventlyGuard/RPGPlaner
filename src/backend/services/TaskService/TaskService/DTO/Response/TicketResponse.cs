using TaskService.DTO.Subticket;
using TaskService.DTO.Tag;
using TaskService.Models;

namespace TaskService.DTO.Response;

public class TicketResponse
{
    public List<SubTicketDto> Subtasks;

    public Guid TicketId { get; set; }
    public string ColumnId { get; set; } 
    public DateTime? StartDate { get; set; } 
    public DateTime? EndDate { get; set; } 
    public string Title { get; set; }
    public string? Description { get; set; } 
    public Priority Priority { get; set; } 
    public Models.Category? Category { get; set; } 
    public Guid UserId { get; set; }
    public TaskType TaskType { get; set; } 
    public int Position { get; set; }
    public bool isComplete {get; set;} = false;
    public string Color { get; set; } = "B3B3B3";
    
    public virtual List<TagDto> Tags { get; set; } = new List<TagDto>();
    public virtual ICollection<SubTicket> SubTickets { get; set; } = null!;
    
    
 



}