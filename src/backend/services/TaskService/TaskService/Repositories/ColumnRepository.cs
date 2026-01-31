using Microsoft.EntityFrameworkCore;
using TaskService.DataAcces;
using TaskService.Models;
using TaskService.Repositories.Interfaces;

namespace TaskService.Repositories;

public class ColumnRepository : IColumnRepository
{
    private readonly TaskContext _context;
    public ColumnRepository(TaskContext context)
    {
        _context = context;
    }
    public async Task<Column?> GetColumn(string columnId, CancellationToken token)
    {
        var column = await _context.Columns.FirstOrDefaultAsync(x => x.ColumnId == columnId, cancellationToken: token);
        return column;
    }

    public async Task<string> CreateColumn(Column column, CancellationToken token)
    {
        var newColumn = new Column
        {
            ColumnId = column.ColumnId,
            Color = column.Color,
            Name = column.Name,
            isAutoComplete = column.isAutoComplete,
            Position = column.Position,
        };
        _context.Columns.Add(newColumn);
        await _context.SaveChangesAsync(token);
        return newColumn.ColumnId;
    }

    public async Task<bool> UpdateColumn(Column column, CancellationToken token)
    {
        var oldColumn = await _context.Columns.FirstOrDefaultAsync(x => x.ColumnId == column.ColumnId, token);
        if (oldColumn == null) return false;
        oldColumn.Name = column.Name;
        oldColumn.Color = column.Color;
        oldColumn.isAutoComplete = column.isAutoComplete;
        oldColumn.Position = column.Position;
        _context.Columns.Update(oldColumn);
        await _context.SaveChangesAsync(token);
        return true;

    }

    public async Task<bool> DeleteColumn(string columnId, CancellationToken token)
    {
        var column = await _context.Columns.FirstOrDefaultAsync(x => x.ColumnId == columnId, token);
        if (column != null) _context.Columns.Remove(column);
        await _context.SaveChangesAsync(token);
        return true;

    }

    public async Task<bool> ChangeColor(string columnId, string newColor, CancellationToken token)
    {
        var column = await _context.Columns.FirstOrDefaultAsync(x=>x.ColumnId ==  columnId, token);
        if (column == null) return false;
        column.Color = newColor;
        _context.Columns.Update(column);
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<IEnumerable<Column>> GetAllColumns(CancellationToken token)
    {
        var  columns = await _context.Columns.ToListAsync(token);
        return columns;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsColumns(string columnId, CancellationToken token)
    {
        var tickets = await _context.Tickets.Where(x=>x.ColumnId==columnId).ToListAsync(token);
        return tickets;
    }
}