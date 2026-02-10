using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TaskService.Models;
using TaskService.Repositories.Interfaces;
using TaskService.Services.Interfaces;

namespace TaskService.Services;

public class TicketService : ITicketService
{
    //Нужно подумать над кешированием, для этого думаю исп редис
    IDistributedCache _cache;
    IColumnRepository _columnRepository;
    ITicketRepository _ticketRepository;
    ITagRepository _tagRepository;

    public TicketService(IDistributedCache cache, IColumnRepository columnRepository,
        ITicketRepository ticketRepository, ITagRepository tagRepository)
    {
        _cache = cache;
        _columnRepository = columnRepository;
        _ticketRepository = ticketRepository;
        _tagRepository = tagRepository;
    }
    public async Task<IEnumerable<Ticket>> GetAllTickets(CancellationToken token)
    {
        var user = await _ticketRepository.GetAllTickets(token);
        return user;
    }

    public async Task<Ticket> GetTicket(Guid ticketId, CancellationToken token)
    {
        Ticket? cahceTicket = null;
        var ticket =  await _ticketRepository.GetTicket(ticketId, token);
        var ticketString = await _cache.GetStringAsync(ticketId.ToString());
        if (ticketString != null) cahceTicket = JsonSerializer.Deserialize<Ticket>(ticketString);
        if (cahceTicket == null)
        {
            cahceTicket = await _ticketRepository.GetTicket(ticketId, token);
            if (cahceTicket != null)
            {
                Console.WriteLine($"Добавили ticket в кэш {cahceTicket}");
                ticketString = JsonSerializer.Serialize(cahceTicket);
                await _cache.SetStringAsync(ticketId.ToString(), ticketString, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                    }
                );
            }
           
        }
        else
        {
            Console.WriteLine($"{ticket.Title} был извлечен из кэша");
        }
        return cahceTicket;

    }

    public async Task<IEnumerable<Ticket>> GetAllColumnTickets(string columnName, CancellationToken token)
    {
        var tickets = await _columnRepository.GetAllTicketsColumns(columnName, token);
        return tickets;
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