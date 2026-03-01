using TaskService.Models;

namespace TaskService.DTO.Tag;
using TaskService.Models;
public class TagDto
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public Category Category { get; set; }
    public int UsageCount { get; set; } 
}