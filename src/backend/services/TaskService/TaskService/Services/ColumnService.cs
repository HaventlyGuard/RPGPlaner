using Microsoft.Extensions.Logging;
using TaskService.Models;
using TaskService.Repositories.Interfaces;
using TaskService.Services.Interfaces;

namespace TaskService.Services;

public class ColumnService : IColumnService
{
    private readonly IColumnRepository _columnRepository;
    private readonly ILogger<ColumnService> _logger;
    private readonly ITicketRepository _ticketRepository;

    public ColumnService(
        IColumnRepository columnRepository,
        ILogger<ColumnService> logger,
        ITicketRepository ticketRepository)
    {
        _columnRepository = columnRepository ?? throw new ArgumentNullException(nameof(columnRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
    }

    public async Task<Column?> GetColumn(string columnId, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(columnId))
        {
            _logger.LogWarning("Attempt to get column with empty ID");
            throw new ArgumentException("Column ID cannot be null or empty", nameof(columnId));
        }

        _logger.LogInformation("Getting column: {ColumnId}", columnId);

        try
        {
            var column = await _columnRepository.GetColumn(columnId, token);
            
            if (column == null)
            {
                _logger.LogWarning("Column not found: {ColumnId}", columnId);
                // Возвращаем null вместо исключения, если это допустимо по контракту
                // Если нужно исключение: throw new KeyNotFoundException($"Column '{columnId}' not found");
            }
            else
            {
                _logger.LogDebug("Retrieved column: {ColumnName} (ID: {ColumnId})", 
                    column.Name, columnId);
            }

            return column;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting column: {ColumnId}", columnId);
            throw;
        }
    }

    public async Task<string> CreateColumn(Column column, CancellationToken token)
    {
        ValidateColumn(column, isNew: true);
        
        _logger.LogInformation("Creating new column: {ColumnName}", column.Name);

        try
        {
            // Проверяем, не существует ли уже колонка с таким ID
            if (!string.IsNullOrEmpty(column.ColumnId))
            {
                var existingColumn = await _columnRepository.GetColumn(column.ColumnId, token);
                if (existingColumn != null)
                {
                    _logger.LogWarning("Column with ID already exists: {ColumnId}", column.ColumnId);
                    throw new InvalidOperationException($"Column with ID '{column.ColumnId}' already exists");
                }
            }

            // Проверяем уникальность имени (опционально, если требуется)
            var allColumns = await _columnRepository.GetAllColumns(token);
            if (allColumns.Any(c => 
                string.Equals(c.Name, column.Name, StringComparison.OrdinalIgnoreCase)))
            {
                _logger.LogWarning("Column with name already exists: {ColumnName}", column.Name);
                throw new InvalidOperationException($"Column with name '{column.Name}' already exists");
            }

            // Автогенерация ID если не указан
            if (string.IsNullOrEmpty(column.ColumnId))
            {
                column.ColumnId = GenerateColumnId(column.Name);
                _logger.LogDebug("Generated column ID: {ColumnId}", column.ColumnId);
            }

            return await _columnRepository.CreateColumn(column, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating column: {ColumnName}", column.Name);
            throw;
        }
    }

    public async Task<bool> UpdateColumn(Column column, CancellationToken token)
    {
        ValidateColumn(column, isNew: false);
        
        if (string.IsNullOrWhiteSpace(column.ColumnId))
        {
            throw new ArgumentException("Column ID is required for update", nameof(column.ColumnId));
        }

        _logger.LogInformation("Updating column: {ColumnId}", column.ColumnId);

        try
        {
            // Проверяем существование колонки
            var existingColumn = await _columnRepository.GetColumn(column.ColumnId, token);
            if (existingColumn == null)
            {
                _logger.LogWarning("Column not found for update: {ColumnId}", column.ColumnId);
                return false; // Или throw new KeyNotFoundException
            }

            // Проверяем уникальность имени (если имя изменилось)
            if (!string.Equals(existingColumn.Name, column.Name, StringComparison.OrdinalIgnoreCase))
            {
                var allColumns = await _columnRepository.GetAllColumns(token);
                if (allColumns.Any(c => 
                    c.ColumnId != column.ColumnId && 
                    string.Equals(c.Name, column.Name, StringComparison.OrdinalIgnoreCase)))
                {
                    _logger.LogWarning("Another column with name already exists: {ColumnName}", column.Name);
                    throw new InvalidOperationException($"Another column with name '{column.Name}' already exists");
                }
            }

            return await _columnRepository.UpdateColumn(column, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating column: {ColumnId}", column.ColumnId);
            throw;
        }
    }

    public async Task<bool> DeleteColumn(string columnId, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(columnId))
        {
            throw new ArgumentException("Column ID cannot be null or empty", nameof(columnId));
        }

        _logger.LogInformation("Deleting column: {ColumnId}", columnId);

        try
        {
            // Проверяем существование колонки
            var existingColumn = await _columnRepository.GetColumn(columnId, token);
            if (existingColumn == null)
            {
                _logger.LogWarning("Column not found for deletion: {ColumnId}", columnId);
                return false; // Или throw new KeyNotFoundException
            }

            // Проверяем, есть ли тикеты в этой колонке
            var ticketsInColumn = await _columnRepository.GetAllTicketsColumns(columnId, token);
            if (ticketsInColumn.Any())
            {
                _logger.LogWarning("Cannot delete column with tickets. Column: {ColumnId}, Tickets count: {Count}", 
                    columnId, ticketsInColumn.Count());
                throw new InvalidOperationException(
                    $"Cannot delete column '{existingColumn.Name}' because it contains {ticketsInColumn.Count()} tickets. " +
                    "Move or delete all tickets first.");
            }

            return await _columnRepository.DeleteColumn(columnId, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting column: {ColumnId}", columnId);
            throw;
        }
    }

    public async Task<bool> ChangeColor(string columnId, string newColor, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(columnId))
        {
            throw new ArgumentException("Column ID cannot be null or empty", nameof(columnId));
        }

        if (string.IsNullOrWhiteSpace(newColor))
        {
            throw new ArgumentException("Color cannot be null or empty", nameof(newColor));
        }

        // Валидация HEX цвета
        if (!IsValidHexColor(newColor))
        {
            throw new ArgumentException("Color must be a valid HEX color (#RRGGBB or #RGB)", nameof(newColor));
        }

        _logger.LogInformation("Changing color for column: {ColumnId} to {NewColor}", columnId, newColor);

        try
        {
            // Проверяем существование колонки
            var existingColumn = await _columnRepository.GetColumn(columnId, token);
            if (existingColumn == null)
            {
                _logger.LogWarning("Column not found for color change: {ColumnId}", columnId);
                return false;
            }

            return await _columnRepository.ChangeColor(columnId, newColor, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing color for column: {ColumnId}", columnId);
            throw;
        }
    }

    public async Task<IEnumerable<Column>> GetAllColumns(CancellationToken token)
    {
        _logger.LogInformation("Getting all columns");

        try
        {
            var columns = await _columnRepository.GetAllColumns(token);
            
            if (!columns.Any())
            {
                _logger.LogInformation("No columns found in the system");
            }
            else
            {
                _logger.LogDebug("Retrieved {Count} columns", columns.Count());
                
                // Сортируем по позиции если нужно
                columns = columns.OrderBy(c => c.Position).ToList();
            }

            return columns;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all columns");
            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsColumns(string columnId, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(columnId))
        {
            throw new ArgumentException("Column ID cannot be null or empty", nameof(columnId));
        }

        _logger.LogInformation("Getting all tickets for column: {ColumnId}", columnId);

        try
        {
            // Проверяем существование колонки
            var column = await _columnRepository.GetColumn(columnId, token);
            if (column == null)
            {
                _logger.LogWarning("Column not found: {ColumnId}", columnId);
                throw new KeyNotFoundException($"Column '{columnId}' not found");
            }

            var tickets = await _columnRepository.GetAllTicketsColumns(columnId, token);
            
            _logger.LogDebug("Retrieved {Count} tickets for column {ColumnId}", 
                tickets.Count(), columnId);

            // Сортируем по позиции если нужно
            return tickets.OrderBy(t => t.Position).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tickets for column: {ColumnId}", columnId);
            throw;
        }
    }

    // Дополнительные методы для бизнес-логики

    public async Task<bool> ReorderColumns(List<string> columnIdsInOrder, CancellationToken token)
    {
        if (columnIdsInOrder == null || !columnIdsInOrder.Any())
        {
            throw new ArgumentException("Column IDs list cannot be null or empty", nameof(columnIdsInOrder));
        }

        _logger.LogInformation("Reordering {Count} columns", columnIdsInOrder.Count);

        try
        {
            // Получаем все колонки для проверки
            var allColumns = await _columnRepository.GetAllColumns(token);
            var allColumnIds = allColumns.Select(c => c.ColumnId).ToList();
            
            // Проверяем, что все указанные ID существуют
            var missingColumns = columnIdsInOrder.Except(allColumnIds).ToList();
            if (missingColumns.Any())
            {
                throw new ArgumentException($"Columns not found: {string.Join(", ", missingColumns)}");
            }

            // Проверяем, что нет лишних колонок (все колонки должны быть в списке)
            var extraColumns = allColumnIds.Except(columnIdsInOrder).ToList();
            if (extraColumns.Any())
            {
                throw new ArgumentException($"Missing columns in order list: {string.Join(", ", extraColumns)}");
            }

            // Обновляем позиции
            for (int i = 0; i < columnIdsInOrder.Count; i++)
            {
                var columnId = columnIdsInOrder[i];
                var column = allColumns.First(c => c.ColumnId == columnId);
                
                if (column.Position != i)
                {
                    column.Position = i;
                    await _columnRepository.UpdateColumn(column, token);
                }
            }

            _logger.LogInformation("Successfully reordered {Count} columns", columnIdsInOrder.Count);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering columns");
            throw;
        }
    }

    public async Task<bool> MoveTicketToColumn(Guid ticketId, string targetColumnId, CancellationToken token)
    {
        if (ticketId == Guid.Empty)
        {
            throw new ArgumentException("Ticket ID cannot be empty", nameof(ticketId));
        }

        if (string.IsNullOrWhiteSpace(targetColumnId))
        {
            throw new ArgumentException("Target column ID cannot be null or empty", nameof(targetColumnId));
        }

        _logger.LogInformation("Moving ticket {TicketId} to column {TargetColumnId}", 
            ticketId, targetColumnId);

        try
        {
            // Проверяем существование тикета
            var ticket = await _ticketRepository.GetTicket(ticketId, token);
            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket '{ticketId}' not found");
            }

            // Проверяем существование целевой колонки
            var targetColumn = await _columnRepository.GetColumn(targetColumnId, token);
            if (targetColumn == null)
            {
                throw new KeyNotFoundException($"Target column '{targetColumnId}' not found");
            }

            // Проверяем, не пытаемся ли переместить в ту же колонку
            if (ticket.ColumnId == targetColumnId)
            {
                _logger.LogWarning("Ticket {TicketId} is already in column {ColumnId}", 
                    ticketId, targetColumnId);
                return false; // Или true, если это допустимо
            }

            // Получаем все тикеты в целевой колонке для определения новой позиции
            var targetColumnTickets = await _columnRepository.GetAllTicketsColumns(targetColumnId, token);
            var newPosition = targetColumnTickets.Any() 
                ? targetColumnTickets.Max(t => t.Position) + 1 
                : 0;

            // Обновляем тикет
            ticket.ColumnId = targetColumnId;
            ticket.Position = newPosition;
            
            await _ticketRepository.UpdateTicket(ticket, token);

            _logger.LogInformation("Successfully moved ticket {TicketId} to column {TargetColumnId} at position {Position}",
                ticketId, targetColumnId, newPosition);
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving ticket {TicketId} to column {TargetColumnId}", 
                ticketId, targetColumnId);
            throw;
        }
    }

    public async Task<bool> SetAutoComplete(string columnId, bool autoComplete, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(columnId))
        {
            throw new ArgumentException("Column ID cannot be null or empty", nameof(columnId));
        }

        _logger.LogInformation("Setting auto-complete to {AutoComplete} for column: {ColumnId}", 
            autoComplete, columnId);

        try
        {
            var column = await _columnRepository.GetColumn(columnId, token);
            if (column == null)
            {
                throw new KeyNotFoundException($"Column '{columnId}' not found");
            }

            column.isAutoComplete = autoComplete;
            return await _columnRepository.UpdateColumn(column, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting auto-complete for column: {ColumnId}", columnId);
            throw;
        }
    }

    // Вспомогательные методы

    private void ValidateColumn(Column column, bool isNew)
    {
        if (column == null)
        {
            throw new ArgumentNullException(nameof(column));
        }

        if (string.IsNullOrWhiteSpace(column.Name))
        {
            throw new ArgumentException("Column name cannot be null or empty", nameof(column.Name));
        }

        if (column.Name.Length > 100)
        {
            throw new ArgumentException("Column name cannot exceed 100 characters", nameof(column.Name));
        }

        if (!isNew && string.IsNullOrWhiteSpace(column.ColumnId))
        {
            throw new ArgumentException("Column ID is required for update", nameof(column.ColumnId));
        }

        if (column.Position < 0)
        {
            throw new ArgumentException("Column position cannot be negative", nameof(column.Position));
        }

        // Валидация цвета если указан
        if (!string.IsNullOrEmpty(column.Color) && !IsValidHexColor(column.Color))
        {
            throw new ArgumentException("Color must be a valid HEX color (#RRGGBB or #RGB)", nameof(column.Color));
        }
    }

    private bool IsValidHexColor(string color)
    {
        if (string.IsNullOrWhiteSpace(color))
            return false;

        // Проверка HEX формата (#RRGGBB или #RGB)
        return System.Text.RegularExpressions.Regex.IsMatch(
            color, 
            "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$");
    }

    private string GenerateColumnId(string columnName)
    {
        // Генерация ID на основе имени и timestamp
        var sanitizedName = columnName
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("_", "-");
        
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        
        return $"{sanitizedName}-{timestamp}-{random}";
    }
}