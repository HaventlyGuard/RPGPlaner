using TaskService.Models;

namespace TaskService.DTO.Deadline;

public class SetDeadlineDto
{
    public DateTime Deadline { get; set; }
}

public class UpdateDeadlineDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class DeadlineInfoDto
{
    public Guid TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime? Deadline { get; set; }
    public bool IsOverdue { get; set; }
    public int? DaysRemaining { get; set; }
    public Priority Priority { get; set; }
}