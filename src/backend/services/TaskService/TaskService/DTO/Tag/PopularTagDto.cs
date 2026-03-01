namespace TaskService.DTO.Tag;

public class PopularTagDto
{
    public TagDto Tag { get; set; } = null!;
    public int UsageCount { get; set; }
    public double UsagePercentage { get; set; }
}