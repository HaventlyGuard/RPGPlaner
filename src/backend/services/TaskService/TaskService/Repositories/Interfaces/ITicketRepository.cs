using TaskService.Models;

namespace TaskService.Repositories.Interfaces;

public interface ITicketRepository
{
    public Task<IEnumerable<Ticket>> GetAllTickets(CancellationToken token);
    public Task<Ticket> GetTicket(Guid ticketId, CancellationToken token);
    public Task<IEnumerable<Ticket>> GetAllColumnTickets(string columnName, CancellationToken token);
    
    public Task<Ticket> UpdateTicket(Ticket ticket, CancellationToken token);
    public Task<Ticket> AddTicket(Ticket ticket, CancellationToken token);
    public Task<bool> DeleteTicket(Guid ticket, CancellationToken token);
    public Task<bool> DragIntoNewColumn( string columnName, Ticket ticket, CancellationToken token);
    
    public Task<string> GetColumnNameColumnByTicketId(Guid ticketId, CancellationToken token); 
    public Task<int> GetTicketPosition(Guid ticketId, CancellationToken token);
    public Task<int> UpdateTicketPosition(Guid ticketId, int taskPosition, CancellationToken token);
    
    public Task<bool> UpdateTicketCategory (Guid ticketId, Category taskCategory, CancellationToken token);
    public Task<bool> DeleteTicketCategory(Guid ticketId, CancellationToken token);
    public Task<bool> AddCategoryToTicket(Guid ticketId, Category taskCategory, CancellationToken token);
    public Task<bool> DeleteCategoryToTicket(Guid ticketId, CancellationToken token);
    
    public Task<bool> AddTagToTicket(Guid ticketId, Tag taskTag, CancellationToken token);
    public Task<bool> DeleteTagFromTicket(Guid ticketId, string taskTag, CancellationToken token);
    public Task<bool> UpdateTagToTicket(Guid ticketId, Tag taskTag, CancellationToken token);
    
    public Task<bool> AddSubTicketToTicket(SubTicket subTicket, CancellationToken token);
    public Task<bool> DeleteSubTicketFromTicket(Guid subTicket, CancellationToken token);
    public Task<bool> UpdateSubTicketToTicket(SubTicket subTicket, CancellationToken token);
    
    public Task<ICollection<Tag>> GetAllTicketTags(Guid ticketId, CancellationToken token);
    public Task<IEnumerable<SubTicket>> GetAllTicketSubTickets(Guid ticketId, CancellationToken token);
    public Task<bool> ChangePriority(Guid ticketId, Priority taskPriority, CancellationToken token);
    
    public Task<bool> AddDeadline(DateTime deadline,  Guid ticketId, CancellationToken token);
    public Task<bool> DeleteDeadline(Guid ticketId, CancellationToken token);
    public Task<bool> UpdateDeadline(DateTime startDay, DateTime endDay, Guid ticketId, CancellationToken token);
    
    public Task<bool> CompleteTicket(Guid ticketId, CancellationToken token);
    
}