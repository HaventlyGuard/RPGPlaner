namespace TaskService.DTO.Tag;
using TaskService.Models;

public class UpdateTagDto
{
    public string? Name { get; set; }
    public string? Color { get; set; }
    public Category? Category { get; set; }
}