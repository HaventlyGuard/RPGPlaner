using Microsoft.AspNetCore.Mvc;
using TaskService.Models;
using TaskService.Services.Interfaces;
using TaskService.DTO;
using TaskService.DTO.Subticket;
using TaskService.DTO.Tag;
using TaskService.DTO.Ticket;
using TaskService.Errors.Exceptions;
using TicketResponse = TaskService.DTO.Response.TicketResponse;

namespace TaskService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITicketService _ticketService;
    private readonly IColumnService _columnService;
    private readonly ITagService _tagService;
    private readonly ILogger<TasksController> _logger;

    public TasksController(
        ITicketService ticketService,
        IColumnService columnService,
        ITagService tagService,
        ILogger<TasksController> logger)
    {
        _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
        _columnService = columnService ?? throw new ArgumentNullException(nameof(columnService));
        _tagService = tagService ?? throw new ArgumentNullException(nameof(tagService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Get all tasks with optional filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TicketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TicketResponse>>> GetAllTasks(
        [FromQuery] string? columnId = null,
        [FromQuery] Priority? priority = null,
        [FromQuery] Category? category = null,
        [FromQuery] bool? isCompleted = null,
        [FromQuery] string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all tasks with filters: Column={ColumnId}, Priority={Priority}, Category={Category}, Completed={IsCompleted}", 
                columnId, priority, category, isCompleted);

            var tickets = await _ticketService.GetAllTickets(cancellationToken);
            
            if (!string.IsNullOrWhiteSpace(columnId))
                tickets = tickets.Where(t => t.ColumnId == columnId);
                
            if (priority.HasValue)
                tickets = tickets.Where(t => t.Priority == priority.Value);
                
            if (category.HasValue)
                tickets = tickets.Where(t => t.Category == category.Value);
                
            if (isCompleted.HasValue)
                tickets = tickets.Where(t => t.isComplete == isCompleted.Value);
                
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchLower = searchTerm.ToLowerInvariant();
                tickets = tickets.Where(t => 
                    t.Title.ToLowerInvariant().Contains(searchLower) ||
                    (t.Description?.ToLowerInvariant().Contains(searchLower) ?? false));
            }

            var ticketDtos = await Task.WhenAll(tickets.Select(async t => await MapToDto(t, cancellationToken)));
            
            return Ok(ticketDtos.OrderBy(t => t.Position));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tasks");
            return StatusCode(500, new { error = "An error occurred while retrieving tasks" });
        }
    }

    /// <summary>
    /// Get task by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketResponse>> GetTask(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting task by ID: {TicketId}", id);
            
            var ticket = await _ticketService.GetTicket(id, cancellationToken);
            if (ticket == null)
                return NotFound(new { error = $"Task with ID {id} not found" });

            var ticketDto = await MapToDto(ticket, cancellationToken);
            return Ok(ticketDto);
        }
        catch (TaskNotFoundExcepion)
        {
            return NotFound(new { error = $"Task with ID {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task {TicketId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the task" });
        }
    }

    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketResponse>> CreateTask([FromBody] CreateTicketDto createDto, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new task: {Title}", createDto.Title);

            // Validate column exists
            var column = await _columnService.GetColumn(createDto.ColumnId, cancellationToken);
            if (column == null)
                return BadRequest(new { error = $"Column with ID {createDto.ColumnId} not found" });

            // Get position for new ticket
            var columnTickets = await _columnService.GetAllTicketsColumns(createDto.ColumnId, cancellationToken);
            var position = columnTickets.Any() ? columnTickets.Max(t => t.Position) + 1 : 0;

            var ticket = new Ticket
            {
                TicketId = Guid.NewGuid(),
                Title = createDto.Title,
                Description = createDto.Description,
                ColumnId = createDto.ColumnId,
                Position = position,
                Priority = createDto.Priority ?? Priority.Normal,
                Category = createDto.Category,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow,
                isComplete = false
            };

            var createdTicket = await _ticketService.AddTicket(ticket, cancellationToken);
            var ticketDto = await MapToDto(createdTicket, cancellationToken);

            return CreatedAtAction(nameof(GetTask), new { id = createdTicket.TicketId }, ticketDto);
        }
        catch (TaskCreateExcepion ex)
        {
            _logger.LogError(ex, "Error creating task");
            return BadRequest(new { error = "Failed to create task" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return StatusCode(500, new { error = "An error occurred while creating the task" });
        }
    }

    /// <summary>
    /// Update a task
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TicketResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TicketResponse>> UpdateTask(Guid id, [FromBody] UpdateTicketDto updateDto, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating task: {TicketId}", id);

            var existingTicket = await _ticketService.GetTicket(id, cancellationToken);
            if (existingTicket == null)
                return NotFound(new { error = $"Task with ID {id} not found" });

            // Update fields
            if (!string.IsNullOrWhiteSpace(updateDto.Title))
                existingTicket.Title = updateDto.Title;
                
            if (updateDto.Description != null)
                existingTicket.Description = updateDto.Description;
                
            if (updateDto.Priority.HasValue)
                existingTicket.Priority = updateDto.Priority.Value;
                
            if (updateDto.Category.HasValue)
                existingTicket.Category = updateDto.Category.Value;


            var updatedTicket = await _ticketService.UpdateTicket(existingTicket, cancellationToken);
            var ticketDto = await MapToDto(updatedTicket, cancellationToken);

            return Ok(ticketDto);
        }
        catch (TaskNotFoundExcepion)
        {
            return NotFound(new { error = $"Task with ID {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TicketId}", id);
            return StatusCode(500, new { error = "An error occurred while updating the task" });
        }
    }

    /// <summary>
    /// Update task status (move to different column)
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTaskStatus(Guid id, [FromBody] UpdateStatusDto statusDto, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating status for task {TicketId} to column {ColumnId}", id, statusDto.ColumnId);

            var ticket = await _ticketService.GetTicket(id, cancellationToken);
            if (ticket == null)
                return NotFound(new { error = $"Task with ID {id} not found" });

            var column = await _columnService.GetColumn(statusDto.ColumnId, cancellationToken);
            if (column == null)
                return BadRequest(new { error = $"Column with ID {statusDto.ColumnId} not found" });

            var success = await _ticketService.DragIntoNewColumn(column.ColumnId, ticket, cancellationToken);
            
            if (!success)
                return BadRequest(new { error = "Failed to update task status" });

            if (column.isAutoComplete)
            {
                await _ticketService.CompleteTicket(id, cancellationToken);
            }

            return Ok(new { message = "Task status updated successfully" });
        }
        catch (TaskNotFoundExcepion)
        {
            return NotFound(new { error = $"Task with ID {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for task {TicketId}", id);
            return StatusCode(500, new { error = "An error occurred while updating task status" });
        }
    }

    /// <summary>
    /// Mark task as completed
    /// </summary>
    [HttpPatch("{id}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CompleteTask(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Completing task: {TicketId}", id);

            var success = await _ticketService.CompleteTicket(id, cancellationToken);
            
            if (!success)
                return NotFound(new { error = $"Task with ID {id} not found" });

            return Ok(new { message = "Task marked as completed" });
        }
        catch (TaskNotFoundExcepion)
        {
            return NotFound(new { error = $"Task with ID {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing task {TicketId}", id);
            return StatusCode(500, new { error = "An error occurred while completing the task" });
        }
    }

    /// <summary>
    /// Update task priority
    /// </summary>
    [HttpPatch("{id}/priority")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdatePriority(Guid id, [FromBody] UpdatePriorityDto priorityDto, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating priority for task {TicketId} to {Priority}", id, priorityDto.Priority);

            var success = await _ticketService.ChangePriority(id, priorityDto.Priority, cancellationToken);
            
            if (!success)
                return NotFound(new { error = $"Task with ID {id} not found" });

            return Ok(new { message = "Task priority updated successfully" });
        }
        catch (TaskNotFoundExcepion)
        {
            return NotFound(new { error = $"Task with ID {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating priority for task {TicketId}", id);
            return StatusCode(500, new { error = "An error occurred while updating task priority" });
        }
    }

    /// <summary>
    /// Delete a task
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTask(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting task: {TicketId}", id);

            var success = await _ticketService.DeleteTicket(id, cancellationToken);
            
            if (!success)
                return NotFound(new { error = $"Task with ID {id} not found" });

            return NoContent();
        }
        catch (TaskCreateExcepion)
        {
            return NotFound(new { error = $"Task with ID {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TicketId}", id);
            return StatusCode(500, new { error = "An error occurred while deleting the task" });
        }
    }

    /// <summary>
    /// Bulk operations on tasks
    /// </summary>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(BulkOperationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BulkOperationResult>> BulkOperations([FromBody] BulkOperationDto bulkDto, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Performing bulk operation: {Operation} on {Count} tasks", 
                bulkDto.Operation, bulkDto.TaskIds?.Count ?? 0);

            if (bulkDto.TaskIds == null || !bulkDto.TaskIds.Any())
                return BadRequest(new { error = "No task IDs provided" });

            var result = new BulkOperationResult
            {
                SuccessCount = 0,
                FailedIds = new List<Guid>(),
                Operation = bulkDto.Operation
            };

            foreach (var taskId in bulkDto.TaskIds)
            {
                try
                {
                    bool success = false;
                    
                    switch (bulkDto.Operation.ToLowerInvariant())
                    {
                        case "delete":
                            success = await _ticketService.DeleteTicket(taskId, cancellationToken);
                            break;
                            
                        case "complete":
                            success = await _ticketService.CompleteTicket(taskId, cancellationToken);
                            break;
                            
                        case "move":
                            if (string.IsNullOrWhiteSpace(bulkDto.TargetColumnId))
                                throw new ArgumentException("Target column ID required for move operation");
                                
                            var ticket = await _ticketService.GetTicket(taskId, cancellationToken);
                            success = await _ticketService.DragIntoNewColumn(bulkDto.TargetColumnId, ticket, cancellationToken);
                            break;
                            
                        case "changepriority":
                            if (!bulkDto.Priority.HasValue)
                                throw new ArgumentException("Priority required for change priority operation");
                                
                            success = await _ticketService.ChangePriority(taskId, bulkDto.Priority.Value, cancellationToken);
                            break;
                            
                        default:
                            return BadRequest(new { error = $"Unknown operation: {bulkDto.Operation}" });
                    }

                    if (success)
                        result.SuccessCount++;
                    else
                        result.FailedIds.Add(taskId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Bulk operation failed for task {TaskId}", taskId);
                    result.FailedIds.Add(taskId);
                }
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk operation");
            return StatusCode(500, new { error = "An error occurred while performing bulk operation" });
        }
    }

    /// <summary>
    /// Add subtask to a task
    /// </summary>
    [HttpPost("{id}/subtasks")]
    [ProducesResponseType(typeof(SubTicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<SubTicketDto>> AddSubtask(Guid id, [FromBody] CreateSubTicketDto createDto, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Adding subtask to task: {TicketId}", id);

            var ticket = await _ticketService.GetTicket(id, cancellationToken);
            if (ticket == null)
                return NotFound(new { error = $"Task with ID {id} not found" });

            var subTicket = new SubTicket
            {
                SubTicketId = Guid.NewGuid(),
                TicketId = id,
                Title = createDto.Title,
                isComplete = false,
            };

            var success = await _ticketService.AddSubTicketToTicket(subTicket, cancellationToken);
            
            if (!success)
                return BadRequest(new { error = "Failed to add subtask" });

            var subTicketDto = new SubTicketDto
            {
                Id = subTicket.SubTicketId,
                Title = subTicket.Title,
                IsCompleted = subTicket.isComplete,
            };

            return CreatedAtAction(nameof(GetTask), new { id }, subTicketDto);
        }
        catch (TaskNotFoundExcepion)
        {
            return NotFound(new { error = $"Task with ID {id} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding subtask to task {TicketId}", id);
            return StatusCode(500, new { error = "An error occurred while adding subtask" });
        }
    }

    /// <summary>
    /// Delete a subtask
    /// </summary>
    [HttpDelete("{taskId}/subtasks/{subtaskId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteSubtask(Guid taskId, Guid subtaskId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Deleting subtask {SubtaskId} from task {TaskId}", subtaskId, taskId);

            var success = await _ticketService.DeleteSubTicketFromTicket(subtaskId, cancellationToken);
            
            if (!success)
                return NotFound(new { error = $"Subtask with ID {subtaskId} not found" });

            return NoContent();
        }
        catch (TaskNotFoundExcepion)
        {
            return NotFound(new { error = $"Task with ID {taskId} not found" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting subtask {SubtaskId} from task {TaskId}", subtaskId, taskId);
            return StatusCode(500, new { error = "An error occurred while deleting subtask" });
        }
    }

    #region Helper Methods

    private async Task<TicketResponse> MapToDto(Ticket ticket, CancellationToken cancellationToken)
    {
        var subtasks = await _ticketService.GetAllTicketSubTickets(ticket.TicketId, cancellationToken);
        var tags = await _ticketService.GetAllTicketTags(ticket.TicketId, cancellationToken);
        
        // Get column name
        string columnName = null;
        try
        {
            var column = await _columnService.GetColumn(ticket.ColumnId, cancellationToken);
            columnName = column?.Name;
        }
        catch { /* Ignore errors in mapping */ }

        return new TicketResponse
        {
            TicketId = ticket.TicketId,
            Title = ticket.Title,
            Description = ticket.Description,
            ColumnId = ticket.ColumnId,
            Position = ticket.Position,
            Priority = ticket.Priority,
            Category = ticket.Category,
            isComplete= ticket.isComplete,
            StartDate = ticket.StartDate,
            EndDate = ticket.EndDate,
            Tags = tags?.Select(t => new TagDto
            {
                Name = t.TagName,
                Color = t.TagColor,
                Category = t.Category
            }).ToList() ?? new List<TagDto>(),
            Subtasks = subtasks?.Select(s => new SubTicketDto
            {
                Id = s.SubTicketId,
                Title = s.Title,
                IsCompleted = s.isComplete,
            }).ToList() ?? new List<SubTicketDto>()
        };
    }

    #endregion
}