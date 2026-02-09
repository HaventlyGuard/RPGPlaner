using TaskService.Models;
using TaskService.Services.Interfaces;

namespace TaskService.Services;

public class TicketService : ITicketService
{
    public async Task<IEnumerable<Ticket>> GetAllTickets(CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<Ticket> GetTicket(Guid ticketId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Ticket>> GetAllColumnTickets(string columnName, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<Ticket> UpdateTicket(Ticket ticket, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<Ticket> AddTicket(Ticket ticket, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteTicket(Guid ticket, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DragIntoNewColumn(string columnName, Ticket ticket, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<string> GetColumnNameColumnByTicketId(Guid ticketId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<int> GetTicketPosition(Guid ticketId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<int> UpdateTicketPosition(Guid ticketId, int taskPosition, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateTicketCategory(Guid ticketId, Category taskCategory, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteTicketCategory(Guid ticketId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddCategoryToTicket(Guid ticketId, Category taskCategory, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteCategoryToTicket(Guid ticketId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddTagToTicket(Guid ticketId, Tag taskTag, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteTagFromTicket(Guid ticketId, string taskTag, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateTagToTicket(Guid ticketId, Tag taskTag, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddSubTicketToTicket(SubTicket subTicket, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteSubTicketFromTicket(Guid subTicket, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateSubTicketToTicket(SubTicket subTicket, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ICollection<Tag>>> GetAllTicketTags(Guid ticketId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<SubTicket>> GetAllTicketSubTickets(Guid ticketId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ChangePriority(Guid ticketId, Priority taskPriority, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> AddDeadline(DateTime deadline, Guid ticketId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteDeadline(Guid ticketId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UpdateDeadline(DateTime startDay, DateTime endDay, Guid ticketId, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CompleteTicket(Guid ticketId, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}