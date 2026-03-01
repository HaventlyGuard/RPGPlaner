using Microsoft.AspNetCore.Mvc;
using TaskService.DTO.Statistic;
using TaskService.DTO.Ticket;
using TaskService.Models;
using TaskService.Services.Interfaces;
using TaskStatisticsDto = TaskService.DTO.Statistic.TaskStatisticsDto;

namespace TaskService.Controllers;

[ApiController]
[Route("api/tasks/[controller]")]
[Produces("application/json")]
public class StatsController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IColumnService _columnService;
    private readonly ILogger<StatsController> _logger;

    public StatsController(
        ITicketService ticketService,
        IColumnService columnService,
        ILogger<StatsController> logger)
    {
        _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
        _columnService = columnService ?? throw new ArgumentNullException(nameof(columnService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get overall task statistics
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TaskStatisticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TaskStatisticsDto>> GetStatistics(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting task statistics");

            var tickets = await _ticketService.GetAllTickets(cancellationToken);
            var ticketsList = tickets.ToList();

            // Исправлено: isComplete вместо IsCompleted
            var stats = new TaskStatisticsDto
            {
                TotalTasks = ticketsList.Count,
                CompletedTasks = ticketsList.Count(t => t.isComplete),
                PendingTasks = ticketsList.Count(t => !t.isComplete),
                
                // Исправлено: используем правильные значения enum Priority
                HighPriorityTasks = ticketsList.Count(t => t.Priority == Priority.High),
                MediumPriorityTasks = ticketsList.Count(t => t.Priority == Priority.Normal), // Medium заменен на Normal
                LowPriorityTasks = ticketsList.Count(t => t.Priority == Priority.Low || t.Priority == Priority.Easy),
                
                TasksByCategory = ticketsList
                    .Where(t => t.Category.HasValue)
                    .GroupBy(t => t.Category.Value)
                    .ToDictionary(g => g.Key.ToString(), g => g.Count()),
                
                CompletionRate = ticketsList.Count > 0 
                    ? Math.Round((double)ticketsList.Count(t => t.isComplete) / ticketsList.Count * 100, 2)
                    : 0,
                
                // Исправлено: используем EndDate для определения просроченных задач
                OverdueTasks = ticketsList.Count(t => 
                    !t.isComplete && 
                    t.EndDate.HasValue && 
                    t.EndDate.Value < DateTime.UtcNow),
                
                // Исправлено: задачи с установленным EndDate
                TasksWithDeadlines = ticketsList.Count(t => t.EndDate.HasValue),
                
                // Исправлено: задачи, заканчивающиеся сегодня
                TasksDueToday = ticketsList.Count(t => 
                    !t.isComplete && 
                    t.EndDate.HasValue && 
                    t.EndDate.Value.Date == DateTime.UtcNow.Date),
                
                // Исправлено: задачи, заканчивающиеся на этой неделе
                TasksDueThisWeek = ticketsList.Count(t => 
                    !t.isComplete && 
                    t.EndDate.HasValue && 
                    t.EndDate.Value >= DateTime.UtcNow && 
                    t.EndDate.Value <= DateTime.UtcNow.AddDays(7)),
                
                AverageSubtasks = ticketsList.Count > 0
                    ? Math.Round(ticketsList.Average(t => 
                    {
                        try
                        {
                            var subtasks = _ticketService.GetAllTicketSubTickets(t.TicketId, cancellationToken).Result;
                            return subtasks?.Count() ?? 0;
                        }
                        catch
                        {
                            return 0;
                        }
                    }), 2)
                    : 0
            };

            var columns = await _columnService.GetAllColumns(cancellationToken);
            stats.TasksByColumn = new Dictionary<string, int>();
            
            foreach (var column in columns)
            {
                var columnTickets = ticketsList.Count(t => t.ColumnId == column.ColumnId);
                stats.TasksByColumn[column.Name] = columnTickets;
            }

            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task statistics");
            return StatusCode(500, new { error = "An error occurred while retrieving statistics" });
        }
    }

    // /// <summary>
    // /// Get daily task statistics for the last 30 days
    // /// </summary>
    // [HttpGet("daily")]
    // [ProducesResponseType(typeof(IEnumerable<DailyStatsDto>), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // public async Task<ActionResult<IEnumerable<DailyStatsDto>>> GetDailyStatistics(
    //     [FromQuery] int days = 30,
    //     CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         _logger.LogInformation("Getting daily statistics for last {Days} days", days);
    //
    //         var tickets = await _ticketService.GetAllTickets(cancellationToken);
    //         var ticketsList = tickets.ToList();
    //
    //         var endDate = DateTime.UtcNow.Date;
    //         var startDate = endDate.AddDays(-days + 1);
    //
    //         var dailyStats = new List<DailyStatsDto>();
    //
    //         for (var date = startDate; date <= endDate; date = date.AddDays(1))
    //         {
    //             // Исправлено: используем StartDate для создания и isComplete с EndDate для завершения
    //             var tasksCreated = ticketsList.Count(t => 
    //                 t.StartDate.HasValue && t.StartDate.Value.Date == date);
    //                 
    //             var tasksCompleted = ticketsList.Count(t => 
    //                 t.isComplete && 
    //                 t.EndDate.HasValue && 
    //                 t.EndDate.Value.Date == date);
    //
    //             dailyStats.Add(new DailyStatsDto
    //             {
    //                 Date = date,
    //                 TasksCreated = tasksCreated,
    //                 TasksCompleted = tasksCompleted,
    //                 NetChange = tasksCreated - tasksCompleted
    //             });
    //         }
    //
    //         return Ok(dailyStats);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting daily statistics");
    //         return StatusCode(500, new { error = "An error occurred while retrieving daily statistics" });
    //     }
    // }

    // /// <summary>
    // /// Get weekly task statistics
    // /// </summary>
    // [HttpGet("weekly")]
    // [ProducesResponseType(typeof(IEnumerable<WeeklyStatsDto>), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // public async Task<ActionResult<IEnumerable<WeeklyStatsDto>>> GetWeeklyStatistics(
    //     [FromQuery] int weeks = 12,
    //     CancellationToken cancellationToken = default)
    // {
    //     try
    //     {
    //         _logger.LogInformation("Getting weekly statistics for last {Weeks} weeks", weeks);
    //
    //         var tickets = await _ticketService.GetAllTickets(cancellationToken);
    //         var ticketsList = tickets.ToList();
    //
    //         var weeklyStats = new List<WeeklyStatsDto>();
    //
    //         var endDate = DateTime.UtcNow.Date;
    //         var startDate = endDate.AddDays(-(weeks * 7) + 1);
    //
    //         for (var weekStart = startDate; weekStart <= endDate; weekStart = weekStart.AddDays(7))
    //         {
    //             var weekEnd = weekStart.AddDays(6).AddHours(23).AddMinutes(59).AddSeconds(59);
    //             
    //             // Исправлено: используем StartDate для создания
    //             var tasksCreated = ticketsList.Count(t => 
    //                 t.StartDate.HasValue && 
    //                 t.StartDate.Value >= weekStart && 
    //                 t.StartDate.Value <= weekEnd);
    //                 
    //             // Исправлено: используем EndDate для завершения
    //             var tasksCompleted = ticketsList.Count(t => 
    //                 t.isComplete && 
    //                 t.EndDate.HasValue && 
    //                 t.EndDate.Value >= weekStart && 
    //                 t.EndDate.Value <= weekEnd);
    //
    //             weeklyStats.Add(new WeeklyStatsDto
    //             {
    //                 WeekStart = weekStart,
    //                 WeekEnd = weekEnd,
    //                 TasksCreated = tasksCreated,
    //                 TasksCompleted = tasksCompleted,
    //                 TotalTasksAtEnd = ticketsList.Count(t => 
    //                     t.StartDate.HasValue && t.StartDate.Value <= weekEnd)
    //             });
    //         }
    //
    //         return Ok(weeklyStats);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting weekly statistics");
    //         return StatusCode(500, new { error = "An error occurred while retrieving weekly statistics" });
    //     }
    // }
    //
    // /// <summary>
    // /// Get task completion rate over time
    // /// </summary>
    // [HttpGet("completion-rate")]
    // [ProducesResponseType(typeof(CompletionRateDto), StatusCodes.Status200OK)]
    // [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    // public async Task<ActionResult<CompletionRateDto>> GetCompletionRate(CancellationToken cancellationToken)
    // {
    //     try
    //     {
    //         _logger.LogInformation("Getting task completion rate");
    //
    //         var tickets = await _ticketService.GetAllTickets(cancellationToken);
    //         var ticketsList = tickets.ToList();
    //
    //         var completedTasks = ticketsList.Count(t => t.isComplete);
    //         var totalTasks = ticketsList.Count;
    //
    //         var rate = new CompletionRateDto
    //         {
    //             TotalTasks = totalTasks,
    //             CompletedTasks = completedTasks,
    //             CompletionRate = totalTasks > 0 ? Math.Round((double)completedTasks / totalTasks * 100, 2) : 0,
    //             
    //             ByPriority = new Dictionary<string, double>(),
    //             ByCategory = new Dictionary<string, double>()
    //         };
    //
    //         // Исправлено: используем все значения Priority из вашей модели
    //         foreach (Priority priority in Enum.GetValues(typeof(Priority)))
    //         {
    //             var tasksWithPriority = ticketsList.Count(t => t.Priority == priority);
    //             var completedWithPriority = ticketsList.Count(t => t.Priority == priority && t.isComplete);
    //             
    //             rate.ByPriority[priority.ToString()] = tasksWithPriority > 0 
    //                 ? Math.Round((double)completedWithPriority / tasksWithPriority * 100, 2)
    //                 : 0;
    //         }
    //
    //         var categories = ticketsList
    //             .Where(t => t.Category.HasValue)
    //             .Select(t => t.Category.Value)
    //             .Distinct();
    //
    //         foreach (var category in categories)
    //         {
    //             var tasksInCategory = ticketsList.Count(t => t.Category == category);
    //             var completedInCategory = ticketsList.Count(t => t.Category == category && t.isComplete);
    //             
    //             rate.ByCategory[category.ToString()] = tasksInCategory > 0 
    //                 ? Math.Round((double)completedInCategory / tasksInCategory * 100, 2)
    //                 : 0;
    //         }
    //
    //         return Ok(rate);
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "Error getting completion rate");
    //         return StatusCode(500, new { error = "An error occurred while retrieving completion rate" });
    //     }
    // }

    /// <summary>
    /// Get overdue tasks (tasks with EndDate in the past and not completed)
    /// </summary>
    [HttpGet("overdue")]
    [ProducesResponseType(typeof(IEnumerable<OverdueTaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<OverdueTaskDto>>> GetOverdueTasks(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting overdue tasks");

            var tickets = await _ticketService.GetAllTickets(cancellationToken);
            var now = DateTime.UtcNow;

            // Исправлено: используем EndDate вместо Deadline
            var overdueTasks = tickets
                .Where(t => !t.isComplete && t.EndDate.HasValue && t.EndDate.Value < now)
                .Select(t => new OverdueTaskDto
                {
                    TaskId = t.TicketId,
                    Title = t.Title,
                    Deadline = t.EndDate.Value, // Маппим EndDate на Deadline в DTO
                    DaysOverdue = (now - t.EndDate.Value).Days,
                    Priority = t.Priority,
                    ColumnId = t.ColumnId
                })
                .OrderByDescending(t => t.DaysOverdue)
                .ToList();

            return Ok(overdueTasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting overdue tasks");
            return StatusCode(500, new { error = "An error occurred while retrieving overdue tasks" });
        }
    }

    /// <summary>
    /// Get upcoming tasks (tasks with EndDate in the next X days)
    /// </summary>
    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(IEnumerable<UpcomingTaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<UpcomingTaskDto>>> GetUpcomingTasks(
        [FromQuery] int days = 7,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting upcoming tasks for next {Days} days", days);

            var tickets = await _ticketService.GetAllTickets(cancellationToken);
            var now = DateTime.UtcNow;
            var cutoff = now.AddDays(days);

            // Исправлено: используем EndDate и isComplete
            var upcomingTasks = tickets
                .Where(t => !t.isComplete && 
                       t.EndDate.HasValue && 
                       t.EndDate.Value > now && 
                       t.EndDate.Value <= cutoff)
                .Select(t => new UpcomingTaskDto
                {
                    TaskId = t.TicketId,
                    Title = t.Title,
                    Deadline = t.EndDate.Value, // Маппим EndDate на Deadline в DTO
                    DaysUntilDeadline = (t.EndDate.Value - now).Days,
                    Priority = t.Priority,
                    ColumnId = t.ColumnId
                })
                .OrderBy(t => t.DaysUntilDeadline)
                .ToList();

            return Ok(upcomingTasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming tasks");
            return StatusCode(500, new { error = "An error occurred while retrieving upcoming tasks" });
        }
    }

    /// <summary>
    /// Get tasks by date range
    /// </summary>
    [HttpGet("by-date-range")]
    [ProducesResponseType(typeof(IEnumerable<TicketSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TicketSummaryDto>>> GetTasksByDateRange(
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting tasks between {StartDate} and {EndDate}", startDate, endDate);

            if (startDate > endDate)
            {
                return BadRequest(new { error = "Start date cannot be after end date" });
            }

            var tickets = await _ticketService.GetAllTickets(cancellationToken);
            
            var tasksInRange = tickets
                .Where(t => t.StartDate.HasValue && 
                       t.StartDate.Value.Date >= startDate.Date && 
                       t.StartDate.Value.Date <= endDate.Date)
                .Select(t => new TicketSummaryDto
                {
                    Id = t.TicketId,
                    Title = t.Title,
                    Priority = t.Priority,
                    Category = t.Category,
                    IsCompleted = t.isComplete,
                    ColumnId = t.ColumnId,
                    Deadline = t.EndDate // Используем EndDate как Deadline
                })
                .OrderBy(t => t.Deadline)
                .ToList();

            return Ok(tasksInRange);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tasks by date range");
            return StatusCode(500, new { error = "An error occurred while retrieving tasks" });
        }
    }
}