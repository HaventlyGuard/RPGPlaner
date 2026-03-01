using Microsoft.AspNetCore.Mvc;
using TaskService.DTO;
using TaskService.DTO.Column;
using TaskService.DTO.Response;
using TaskService.Models;
using TaskService.Services.Interfaces;

namespace TaskService.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class ColumnController : ControllerBase
{
    private ITicketService TicketService { get; }
    private ILogger<ColumnController> Logger { get; }
    private IColumnService ColumnService { get; }
    private ITagService TagService { get; }
    
    public ColumnController(ILogger<ColumnController> logger, IColumnService columnService, ITagService tagService,  ITicketService ticketService)
    {
        Logger = logger;
        ColumnService = columnService;
        TagService = tagService;
        TicketService = ticketService;
    }
    
    /// <summary>
    /// Get all columns
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ColumnDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ColumnDto>>> GetAllColumns(CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Getting all columns");
            
            var columns = await ColumnService.GetAllColumns(cancellationToken);
            var columnDtos = columns.Select(MapToDto);
            
            return Ok(columnDtos);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting all columns");
            return StatusCode(500, new { error = "An error occurred while retrieving columns" });
        }
    }
    
    /// <summary>
    /// Get column by ID
    /// </summary>
    [HttpGet("{columnId}")]
    [ProducesResponseType(typeof(ColumnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ColumnDto>> GetColumn(string columnId, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Getting column: {ColumnId}", columnId);
            
            var column = await ColumnService.GetColumn(columnId, cancellationToken);
            if (column == null)
                return NotFound(new { error = $"Column with ID {columnId} not found" });
            
            return Ok(MapToDto(column));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting column {ColumnId}", columnId);
            return StatusCode(500, new { error = "An error occurred while retrieving column" });
        }
    }
    
    /// <summary>
    /// Create a new column
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ColumnDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ColumnDto>> CreateColumn([FromBody] CreateColumnDto createDto, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Creating new column: {ColumnName}", createDto.Name);
            
            var column = new Column
            {
                Name = createDto.Name,
                Color = createDto.Color ?? "#B3B3B3",
                isAutoComplete = createDto.IsAutoComplete,
                Position = createDto.Position,
            };
            
            var columnId = await ColumnService.CreateColumn(column, cancellationToken);
            var createdColumn = await ColumnService.GetColumn(columnId, cancellationToken);
            
            return CreatedAtAction(nameof(GetColumn), new { columnId }, MapToDto(createdColumn));
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogWarning(ex, "Invalid operation creating column");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating column");
            return StatusCode(500, new { error = "An error occurred while creating column" });
        }
    }
    
    /// <summary>
    /// Update a column
    /// </summary>
    [HttpPut("{columnId}")]
    [ProducesResponseType(typeof(ColumnDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ColumnDto>> UpdateColumn(string columnId, [FromBody] UpdateColumnDto updateDto, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Updating column: {ColumnId}", columnId);
            
            var existingColumn = await ColumnService.GetColumn(columnId, cancellationToken);
            if (existingColumn == null)
                return NotFound(new { error = $"Column with ID {columnId} not found" });
            
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
                existingColumn.Name = updateDto.Name;
                
            if (!string.IsNullOrWhiteSpace(updateDto.Color))
                existingColumn.Color = updateDto.Color;
                
            if (updateDto.Position.HasValue)
                existingColumn.Position = updateDto.Position.Value;
                
            if (updateDto.IsAutoComplete.HasValue)
                existingColumn.isAutoComplete = updateDto.IsAutoComplete.Value;
            
            var success = await ColumnService.UpdateColumn(existingColumn, cancellationToken);
            
            if (!success)
                return NotFound(new { error = $"Column with ID {columnId} not found" });
            
            var updatedColumn = await ColumnService.GetColumn(columnId, cancellationToken);
            return Ok(MapToDto(updatedColumn));
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogWarning(ex, "Invalid operation updating column");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating column {ColumnId}", columnId);
            return StatusCode(500, new { error = "An error occurred while updating column" });
        }
    }
    
    /// <summary>
    /// Delete a column
    /// </summary>
    [HttpDelete("{columnId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteColumn(string columnId, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Deleting column: {ColumnId}", columnId);
            
            var success = await ColumnService.DeleteColumn(columnId, cancellationToken);
            
            if (!success)
                return NotFound(new { error = $"Column with ID {columnId} not found" });
            
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            Logger.LogWarning(ex, "Invalid operation deleting column");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting column {ColumnId}", columnId);
            return StatusCode(500, new { error = "An error occurred while deleting column" });
        }
    }
    
    /// <summary>
    /// Get all tickets in a column
    /// </summary>
    [HttpGet("{columnId}/tickets")]
    [ProducesResponseType(typeof(IEnumerable<TicketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<TicketResponse>>> GetColumnTickets(string columnId, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Getting tickets for column: {ColumnId}", columnId);
            
            var column = await ColumnService.GetColumn(columnId, cancellationToken);
            if (column == null)
                return NotFound(new { error = $"Column with ID {columnId} not found" });
            
            var tickets = await ColumnService.GetAllTicketsColumns(columnId, cancellationToken);
            var ticketDtos = tickets.Select(t => new TicketResponse
            {
                TicketId = t.TicketId,
                Title = t.Title,
                Description = t.Description,
                ColumnId = t.ColumnId,
                Position = t.Position,
                Priority = t.Priority,
                Category = t.Category,
                isComplete = t.isComplete,
                StartDate = t.StartDate,
                EndDate = t.EndDate
            });
            
            return Ok(ticketDtos);
        }
        catch (KeyNotFoundException ex)
        {
            Logger.LogWarning(ex, "Column not found");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting tickets for column {ColumnId}", columnId);
            return StatusCode(500, new { error = "An error occurred while retrieving tickets" });
        }
    }
    
    /// <summary>
    /// Change column color
    /// </summary>
    [HttpPatch("{columnId}/color")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ChangeColor(string columnId, [FromBody] ChangeColumnColorDto colorDto, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Changing color for column: {ColumnId} to {Color}", columnId, colorDto.Color);
            
            var success = await ColumnService.ChangeColor(columnId, colorDto.Color, cancellationToken);
            
            if (!success)
                return NotFound(new { error = $"Column with ID {columnId} not found" });
            
            return Ok(new { message = "Column color updated successfully" });
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning(ex, "Invalid color format");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error changing color for column {ColumnId}", columnId);
            return StatusCode(500, new { error = "An error occurred while changing column color" });
        }
    }
    
  
    
    /// <summary>
    /// Reorder columns
    /// </summary>
    [HttpPost("reorder")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ReorderColumns([FromBody] ReorderColumnsDto reorderDto, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Reordering {Count} columns", reorderDto.ColumnIds.Count);
            
            var success = await ColumnService.ReorderColumns(reorderDto.ColumnIds, cancellationToken);
            
            return Ok(new { message = "Columns reordered successfully" });
        }
        catch (ArgumentException ex)
        {
            Logger.LogWarning(ex, "Invalid reorder request");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reordering columns");
            return StatusCode(500, new { error = "An error occurred while reordering columns" });
        }
    }
    
    /// <summary>
    /// Move ticket to another column
    /// </summary>
    [HttpPost("move-ticket")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MoveTicketToColumn([FromBody] MoveTicketDto moveDto, CancellationToken cancellationToken)
    {
        try
        {
            Logger.LogInformation("Moving ticket {TicketId} to column {TargetColumnId}", 
                moveDto.TicketId, moveDto.TargetColumnId);
            
            var ticket = await TicketService.GetTicket(moveDto.TicketId, cancellationToken);
            var success = await TicketService.DragIntoNewColumn(moveDto.TargetColumnId,  ticket, cancellationToken);
            
            return Ok(new { message = "Ticket moved successfully" });
        }
        catch (KeyNotFoundException ex)
        {
            Logger.LogWarning(ex, "Ticket or column not found");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error moving ticket");
            return StatusCode(500, new { error = "An error occurred while moving ticket" });
        }
    }
    
    private ColumnDto MapToDto(Column column)
    {
        return new ColumnDto
        {
            Id = column.ColumnId,
            Name = column.Name,
            Color = column.Color,
            Position = column.Position,
            IsAutoComplete = column.isAutoComplete
        };
    }
}