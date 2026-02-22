using System.Data;
using System.Runtime.InteropServices.JavaScript;
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
            await _cache.SetStringAsync(addedTicket.TicketId.ToString(), addedTicket.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            },token);
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
         await _cache.RemoveAsync(ticket.ToString(), token);
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
                var ticket = await _ticketRepository.GetTicket(ticketId, token);
                var serilize =   JsonSerializer.Serialize<Ticket>(ticket);
                await _cache.SetStringAsync(ticketId.ToString(),serilize,token);
            }
            return await _ticketRepository.UpdateTicketPosition(ticketId, taskPosition, token);
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to update ticket position excepion, {e}");
            throw new TaskNotFoundExcepion();
        }
    }

    public async Task<bool> UpdateTicketCategory(Guid ticketId, Category taskCategory, CancellationToken token)
    {
        try
        {
            return await _ticketRepository.UpdateTicketCategory(ticketId, taskCategory, token);
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to update ticket position excepion, {e}");
            throw new TaskNotFoundExcepion();
        }
    }

    public async Task<bool> DeleteTicketCategory(Guid ticketId, CancellationToken token)
    {
        try
        {
            return await _ticketRepository.DeleteCategoryToTicket(ticketId, token);
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to delete ticket category excepion, {e}");
            throw new TaskNotFoundExcepion();
        }
    }

    public async Task<bool> AddCategoryToTicket(Guid ticketId, Category taskCategory, CancellationToken token)
    {
        try
        {   
            
            return await _ticketRepository.UpdateTicketCategory(ticketId, taskCategory, token);
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to update ticket position excepion, {e}");
            throw new TaskNotFoundExcepion();
        }
    }

    public async Task<bool> DeleteCategoryToTicket(Guid ticketId, CancellationToken token)
    {
        try
        {
            if (_cache.GetString(ticketId.ToString()) != null) await _cache.RemoveAsync(ticketId.ToString(), token);
            return await _ticketRepository.DeleteCategoryToTicket(ticketId, token);
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to delete ticket category excepion, {e}");
            throw new TaskNotFoundExcepion();
        }
    }

    public async Task<bool> AddTagToTicket(Guid ticketId, Tag taskTag, CancellationToken token)
    {
        try
        {
            var isAdd= await _ticketRepository.AddTagToTicket(ticketId, taskTag, token);
            await _cache.RemoveAsync(ticketId.ToString(), token);
            return isAdd;
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to add tag to ticket exception, {e}");
            throw new TaskNotFoundExcepion();
        }
    }

    public async Task<bool> DeleteTagFromTicket(Guid ticketId, string taskTag, CancellationToken token)
    {
        try
        {
            var isDelete = await _ticketRepository.DeleteTagFromTicket(ticketId, taskTag, token);
            await _cache.RemoveAsync(ticketId.ToString(), token);
            return isDelete;
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to delete tag from ticket exception, {e}");
            throw new TaskNotFoundExcepion();
        }
    }

    public async Task<bool> UpdateTagToTicket(Guid ticketId, Tag taskTag, CancellationToken token)
    {
        try
        {
            var isUpdate = await _ticketRepository.UpdateTagToTicket(ticketId, taskTag, token);
            await _cache.RemoveAsync(ticketId.ToString(), token);
            return isUpdate;
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to update tag to ticket exception, {e}");
            throw new TaskNotFoundExcepion();
        }
    }

    public async Task<bool> AddSubTicketToTicket(SubTicket subTicket, CancellationToken token)
    {
        try
        {
            var subticketIsAdded = await _ticketRepository.AddSubTicketToTicket(subTicket, token);
            await _cache.RemoveAsync(subTicket.TicketId.ToString(), token);
            return subticketIsAdded;
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to add sub ticket to ticket exception, {e}");
            throw new  TaskNotFoundExcepion();
        }
       
    }

    public async Task<bool> DeleteSubTicketFromTicket(Guid subTicket, CancellationToken token)
    {
        try
        {
            var subticketIsRemoved = await _ticketRepository.DeleteSubTicketFromTicket(subTicket, token);
            return (subticketIsRemoved);
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to delete sub ticket from ticket exception, {e}");
            throw new  TaskNotFoundExcepion();
        }
       
    }

    public async Task<bool> UpdateSubTicketToTicket(SubTicket subTicket, CancellationToken token)
    {
        try
        {
            var subticketIsUpdated = await _ticketRepository.UpdateSubTicketToTicket(subTicket, token);
            await _cache.RemoveAsync(subTicket.TicketId.ToString(), token);
            return subticketIsUpdated;
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to update sub ticket to ticket exception, {e}");
            throw new  TaskNotFoundExcepion();
        }
    }

    public async Task<ICollection<Tag>> GetAllTicketTags(Guid ticketId, CancellationToken token)
    {
        try
        {
          
            var tagString =  await _cache.GetStringAsync(ticketId.ToString(), token);
            if (tagString == null)
            {
                var tags = await _ticketRepository.GetAllTicketTags(ticketId, token);
                var serialize =  JsonSerializer.Serialize(tags);
                
                await _cache.SetStringAsync(ticketId.ToString() + "tags", serialize, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                }, token);
                return tags;
            }
            var cahchedTags = await _cache.GetAsync(ticketId.ToString() + "tags", token);
            var deserialize =  JsonSerializer.Deserialize<ICollection<Tag>>(cahchedTags);
                return deserialize;
            
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to get all ticket tags exception, {e}");
            throw new  TaskNotFoundExcepion();
        }
    }

    public async Task<IEnumerable<SubTicket>> GetAllTicketSubTickets(Guid ticketId, CancellationToken token)
    {
        try
        {
          
            var subticketString =  await _cache.GetStringAsync(ticketId.ToString(), token);
            if (subticketString == null)
            {
                var subtickets = await _ticketRepository.GetAllTicketSubTickets(ticketId, token);
                var serialize =  JsonSerializer.Serialize(subtickets);
                
                await _cache.SetStringAsync(ticketId.ToString() + "subtickets", serialize, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                }, token);
                return subtickets;
            }
            var cachedSubTickets = await _cache.GetAsync(ticketId.ToString() + "subtickets", token);
            var deserialize =  JsonSerializer.Deserialize<IEnumerable<SubTicket>>(cachedSubTickets);
            return deserialize;
            
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to get all ticket tags exception, {e}");
            throw new  TaskNotFoundExcepion();
        }
    }

    public async Task<bool> ChangePriority(Guid ticketId, Priority taskPriority, CancellationToken token)
    {
        try
        {
            var priorityIsChanged = await _ticketRepository.ChangePriority(ticketId, taskPriority, token);
            await _cache.RemoveAsync(ticketId.ToString(), token);
            return priorityIsChanged;
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to change priority exception, {e}");
            throw new  TaskNotFoundExcepion();
        }
    }

    public async Task<bool> AddDeadline(DateTime deadline, Guid ticketId, CancellationToken token)
    {
        try
        {
            var addDeadline = await _ticketRepository.AddDeadline(deadline,ticketId, token);
            await _cache.RemoveAsync(ticketId.ToString(), token);
            return addDeadline;
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to add deadline exception, {e}");
            throw new  TaskNotFoundExcepion();
        }
    }

    public async Task<bool> DeleteDeadline(Guid ticketId, CancellationToken token)
    {
        try
        {
            var deletedDeadline = await _ticketRepository.DeleteDeadline(ticketId, token);
            await _cache.RemoveAsync(ticketId.ToString(), token);
            return deletedDeadline;
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to delete deadline exception, {e}");
            throw new  TaskNotFoundExcepion();
        }
    }

    public async Task<bool> UpdateDeadline(DateTime startDay, DateTime endDay, Guid ticketId, CancellationToken token)
    {
        try
        {
            var updatedDeadline = await _ticketRepository.UpdateDeadline(startDay, endDay, ticketId, token);
            await _cache.RemoveAsync(ticketId.ToString(), token);
            return updatedDeadline;
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to update deadline exception, {e}");
            throw new  TaskNotFoundExcepion();
        }
    }

    public async Task<bool> CompleteTicket(Guid ticketId, CancellationToken token)
    {
        try
        {
            var isComplete =  await _ticketRepository.CompleteTicket(ticketId, token);
            await _cache.RemoveAsync(ticketId.ToString(), token);
            return isComplete;
        }
        catch (AppException e)
        {
            _logger.LogError($"Try to complete task exception, {e}");
            throw new  TaskNotFoundExcepion();
        }
    }
}