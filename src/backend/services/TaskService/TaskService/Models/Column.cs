namespace TaskService.Models;

public class Column
{
    public string ColumnId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool isAutoComplete { get; set; } = false;
    public string Color { get; set; } = "B3B3B3";
    
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}