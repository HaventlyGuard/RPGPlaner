namespace TaskService.DTO.Column;

public class UpdateColumnDto
{
    public string? Name { get; set; }
    public int? Position { get; set; }
    public bool? IsAutoComplete { get; set; }
    public string? Color { get; set; }
}