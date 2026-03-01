namespace TaskService.DTO.Column;

public class CreateColumnDto
{
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool IsAutoComplete { get; set; } = false;
    public string Color { get; set; } = "B3B3B3";
}