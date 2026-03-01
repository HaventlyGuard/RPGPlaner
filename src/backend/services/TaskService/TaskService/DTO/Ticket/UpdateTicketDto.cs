using TaskService.Models;

namespace TaskService.DTO.Ticket;
using TaskService.Models;
public class UpdateTicketDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Priority? Priority { get; set; }
    public Category? Category { get; set; }
    public TaskType? TaskType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Color { get; set; }
}