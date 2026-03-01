using TaskService.Models;

namespace TaskService.DTO;
using TaskService.Models;
public class EnumValueDto
{
    public int Value { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
}

public class SystemEnumsDto
{
    public List<EnumValueDto> Priorities { get; set; } = Enum.GetValues<Priority>()
        .Select(p => new EnumValueDto
        {
            Value = (int)p,
            Name = p.ToString(),
            DisplayName = GetPriorityDisplayName(p)
        }).ToList();

    public List<EnumValueDto> Categories { get; set; } = Enum.GetValues<Models.Category>()
        .Select(c => new EnumValueDto
        {
            Value = (int)c,
            Name = c.ToString()
        }).ToList();

    public List<EnumValueDto> TaskTypes { get; set; } = Enum.GetValues<TaskType>()
        .Select(t => new EnumValueDto
        {
            Value = (int)t,
            Name = t.ToString(),
            DisplayName = GetTaskTypeDisplayName(t)
        }).ToList();

    private static string GetPriorityDisplayName(Priority priority) => priority switch
    {
        Priority.Easy => "Easy",
        Priority.Low => "Low",
        Priority.Normal => "Normal",
        Priority.High => "High",
        Priority.Critical => "Critical",
        _ => priority.ToString()
    };

    private static string GetTaskTypeDisplayName(TaskType taskType) => taskType switch
    {
        TaskType.Kanban => "Kanban Task",
        TaskType.CheckBox => "Checklist Task",
        TaskType.FileCheck => "File Review Task",
        TaskType.LineTask => "Linear Task",
        _ => taskType.ToString()
    };
}