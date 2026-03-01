using TaskService.Models;

namespace TaskService.DTO.Statistic;
using TaskService.Models;


public class TaskStatisticsDto
{
    // Общие показатели
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int PendingTasks { get; set; }
    public double CompletionRate { get; set; }
    
    // По приоритетам
    public int HighPriorityTasks { get; set; }
    public int MediumPriorityTasks { get; set; }
    public int LowPriorityTasks { get; set; }
    public int CriticalPriorityTasks { get; set; }
    public int EasyPriorityTasks { get; set; }
    
    // Распределение по категориям
    public Dictionary<string, int> TasksByCategory { get; set; } = new();
    
    // Распределение по колонкам
    public Dictionary<string, int> TasksByColumn { get; set; } = new();
    
    // Срочные задачи (на основе EndDate)
    public int OverdueTasks { get; set; }
    public int TasksWithDeadlines { get; set; }
    public int TasksDueToday { get; set; }
    public int TasksDueThisWeek { get; set; }
    
    // Подзадачи
    public int TotalSubtasks { get; set; }
    public int CompletedSubtasks { get; set; }
    public double AverageSubtasks { get; set; }
}

public class OverdueTaskDto
{
    public Guid TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Deadline { get; set; } // Маппится из EndDate
    public int DaysOverdue { get; set; }
    public Priority Priority { get; set; }
    public string ColumnId { get; set; } = string.Empty;
}

public class UpcomingTaskDto
{
    public Guid TaskId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateTime Deadline { get; set; } // Маппится из EndDate
    public int DaysUntilDeadline { get; set; }
    public Priority Priority { get; set; }
    public string ColumnId { get; set; } = string.Empty;
}