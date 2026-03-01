namespace TaskService.DTO.Category;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public int CompletedCount { get; set; }
    public double CompletionRate => TaskCount > 0 ? Math.Round((double)CompletedCount / TaskCount * 100, 2) : 0;
}

