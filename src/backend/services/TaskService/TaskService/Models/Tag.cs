namespace TaskService.Models;

public class Tag
{
    public string TagName { get; set; }
    public string TagColor { get; set; }
    public Category Category { get; set; }
    
}