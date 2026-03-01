namespace TaskService.DTO.Column;

public class MoveTicketDto
{
    public Guid TicketId { get; set; }
    public string TargetColumnId { get; set; } = string.Empty;
    public int? NewPosition { get; set; }
}