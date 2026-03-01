namespace TaskService.DTO.Ticket;

public class BulkOperationResult
{
    public int SuccessCount { get; set; }
    public List<Guid> FailedIds { get; set; } = new();
    public string Operation { get; set; } = string.Empty;
}