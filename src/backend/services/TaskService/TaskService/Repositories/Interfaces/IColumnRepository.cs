using TaskService.Models;

namespace TaskService.Repositories.Interfaces;

public interface IColumnRepository
{
    public Task<Column?> GetColumn(string columnId, CancellationToken token);
    public Task<string> CreateColumn(Column column, CancellationToken token);
    public Task<bool> UpdateColumn(Column column, CancellationToken token);
    public Task<bool> DeleteColumn(string columnId, CancellationToken token);
    
    public Task<bool> ChangeColor(string columnId, string newColor, CancellationToken token);
    
    public Task<IEnumerable<Column>> GetAllColumns(CancellationToken token);
    public Task<IEnumerable<Ticket>> GetAllTicketsColumns(string columnId, CancellationToken token);
    
}