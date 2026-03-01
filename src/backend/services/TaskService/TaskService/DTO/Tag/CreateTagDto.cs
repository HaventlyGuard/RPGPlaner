using TaskService.Models;

namespace TaskService.DTO.Tag;
using TaskService.Models;
public class CreateTagDto
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#B3B3B3";
    public Category Category { get; set; }
}