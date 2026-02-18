using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TaskService.Errors.Exceptions;
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
    ILogger<TicketService> _logger;

    public TicketService(IDistributedCache cache, IColumnRepository columnRepository,
        ITicketRepository ticketRepository, ITagRepository tagRepository, ILogger<TicketService> logger)
    {
        _cache = cache;
        _columnRepository = columnRepository;
        _ticketRepository = ticketRepository;
        _tagRepository = tagRepository;
        _logger = logger;
    }
    public async Task<IEnumerable<Ticket>> GetAllTickets(CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Try to get add all tickets");
            var user = await _ticketRepository.GetAllTickets(token);
            return user;
        }
        catch (TaskNotFoundExcepion e)
        {
            _logger.LogError(e.Message);
            throw new TaskNotFoundExcepion();
        }
        
       
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
        try
        {
            _logger.LogInformation("Try to update ticket");
            Ticket? cacheTicket = null;
            var ticketString = await _cache.GetStringAsync(ticket.TicketId.ToString());
            var updateTicket = await  _ticketRepository.UpdateTicket(ticket, token);
            if (ticketString != null)
            {
                _logger.LogInformation("update ticket in cache");
                await _cache.RefreshAsync(ticket.TicketId.ToString(), token);
            }
            else
            {
                _logger.LogInformation("add ticket in cache");
                await _cache.SetStringAsync(ticket.TicketId.ToString(), ticketString,
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) }, token);

            }
            return updateTicket;
        }
        catch (AppException e)
        {
            _logger.LogError(e.Message);
            throw new TaskNotFoundExcepion();
        }
       
    }

    public async Task<Ticket> AddTicket(Ticket ticket, CancellationToken token)
    {
        try
        { 
            _logger.LogInformation("Try to add ticket"); 
           var addedTicket = await _ticketRepository.AddTicket(ticket, token);
            await _cache.SetStringAsync(addedTicket.TicketId.ToString(), addedTicket.ToString(), token);
            return addedTicket;
        }
        catch (AppException e)
        {
            _logger.LogError(e.Message);
            throw new TaskCreateExcepion();
        }
    }

    public async Task<bool> DeleteTicket(Guid ticket, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Try to delete ticket");
            var isDeleted = await _ticketRepository.DeleteTicket(ticket, token);
            if (isDeleted)
            {
                await _cache.RemoveAsync(ticket.ToString(), token);
            }
            return isDeleted;
        }
        catch (AppException e)
        {
            _logger.LogError("Delete ticket excepion");
            throw new TaskCreateExcepion();
        }

    }

    public async Task<bool> DragIntoNewColumn(string newColumnName, Ticket ticket, CancellationToken token)
    {
        try
        {
         var isTicketDrag = await _ticketRepository.DragIntoNewColumn(newColumnName, ticket, token);
         await _cache.RefreshAsync(ticket.TicketId.ToString(), token);
         return isTicketDrag;

        }
        catch (AppException e)
        {
            _logger.LogError($"DragIntoNewColumn excepion, {e}");
            throw new ColumnNotFoundExcepion();
        }
    }

    public async Task<string> GetColumnNameColumnByTicketId(Guid ticketId, CancellationToken token)
    {
        try
        {
            var columnName = await _ticketRepository.GetColumnNameColumnByTicketId(ticketId, token);
            return columnName;
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to get column name excepion, {e}");
            throw new ColumnNotFoundExcepion();
        }
    }

    public async Task<int> GetTicketPosition(Guid ticketId, CancellationToken token)
    {
        try
        {
            Ticket? cahceTicket = null;
            var ticketString = await _cache.GetStringAsync(ticketId.ToString());
            if (ticketString != null)
            {
                _logger.LogInformation("get ticket position in cache");
                cahceTicket = JsonSerializer.Deserialize<Ticket>(ticketString);
                return cahceTicket.Position;
            }
            var position = await _ticketRepository.GetTicketPosition(ticketId, token);
            _logger.LogInformation("add ticket position in cache");
            await _cache.SetStringAsync(ticketId.ToString(), position.ToString(),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                }, token);
            return position;
            
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to get ticket position excepion, {e}");
            throw new TaskNotFoundExcepion();
        }
    }

    public async Task<int> UpdateTicketPosition(Guid ticketId, int taskPosition, CancellationToken token)
    {
        try
        {
            var ticketString = await  _cache.GetStringAsync(ticketId.ToString());
            if (ticketString != null)
            {
               await _cache.RefreshAsync(ticketId.ToString(), token);
            }
            return await _ticketRepository.UpdateTicketPosition(ticketId, taskPosition, token);
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to get ticket position excepion, {e}");
            throw new TaskNotFoundExcepion();
        }
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