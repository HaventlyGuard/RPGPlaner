namespace TaskService.Models;

public class Column
{
    public string ColumnId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool isAutoComplete { get; set; } = false;
    
}