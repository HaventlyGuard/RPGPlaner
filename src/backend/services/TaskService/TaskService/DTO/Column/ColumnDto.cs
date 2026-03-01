using TaskService.DTO.Ticket;

namespace TaskService.DTO.Column;

public class ColumnDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool IsAutoComplete { get; set; }
    public string Color { get; set; } = "B3B3B3";
    public int TicketsCount { get; set; }
    public List<TicketSummaryDto>? Tickets { get; set; }
}

