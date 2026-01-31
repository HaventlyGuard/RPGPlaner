using Microsoft.EntityFrameworkCore;
using TaskService.DataAcces;
using TaskService.Models;
using TaskService.Repositories.Interfaces;

namespace TaskService.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly TaskContext _context;
    private ITicketRepository _ticketRepositoryImplementation;

    public TicketRepository(TaskContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ticket>> GetAllTickets(CancellationToken token)
    {
        var tickets = await _context.Tickets.ToListAsync(token);
        return tickets;
    }

    public async Task<Ticket> GetTicket(Guid ticketId, CancellationToken token)
    {
        var ticket = await _context.Tickets
            .FirstOrDefaultAsync(x => x.TicketId == ticketId, cancellationToken: token);
        return ticket;
    }

    public async Task<IEnumerable<Ticket>> GetAllColumnTickets(string columnName, CancellationToken token)
    {
        var tickets = await _context.Tickets
            .Where(x=> x.ColumnId == columnName)
            .ToListAsync(token);
        return tickets;
    }

    public async Task<Ticket> UpdateTicket(Ticket ticket, CancellationToken token)
    {
        var task = await _context.Tickets.FirstOrDefaultAsync(x=>x.TicketId==ticket.TicketId, token);
        task.ColumnId = ticket.ColumnId;
        task.Description = ticket.Description;
        task.Category = ticket.Category;
        task.Color = ticket.Color;
        task.Position = ticket.Position;
        task.Priority = ticket.Priority;
        task.Title = ticket.Title;
        task.TaskType = ticket.TaskType;
        task.Tags = ticket.Tags;
        task.isComplete = ticket.isComplete;
        task.EndDate = ticket.EndDate;
        task.SubTickets = ticket.SubTickets;
        
        await _context.SaveChangesAsync(token);
        return task;
    }

    public async Task<Ticket> AddTicket(Ticket ticket, CancellationToken token)
    {
        var newTicket = new Ticket
        {
            ColumnId = ticket.ColumnId,
            Description = ticket.Description,
            Category = ticket.Category,
            Color = ticket.Color,
            Position = ticket.Position,
            Priority = ticket.Priority,
            Title = ticket.Title,
            TaskType = ticket.TaskType,
            Tags = ticket.Tags,
            isComplete = ticket.isComplete,
            StartDate = DateTime.Now,
            EndDate = ticket.EndDate,
            SubTickets = ticket.SubTickets,

        };
        _context.Tickets.Add(newTicket);
        await _context.SaveChangesAsync(token);
        return newTicket;
    }

    public async Task<bool> DeleteTicket(Guid ticket, CancellationToken token)
    {
        var task = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticket, token);
        _context.Tickets.Remove(task);
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> DragIntoNewColumn(string columnName, Ticket ticket, CancellationToken token)
    {
        ticket.ColumnId =  columnName;
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<string?> GetColumnNameColumnByTicketId(Guid ticketId, CancellationToken token)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        return ticket?.ColumnId;
    }

    public async Task<int> GetTicketPosition(Guid ticketId, CancellationToken token)
    {
        var ticket =  await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        return ticket?.Position ?? 0;
    }

    public async Task<int> UpdateTicketPosition(Guid ticketId, int taskPosition, CancellationToken token)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        ticket.Position = taskPosition;
        await _context.SaveChangesAsync(token);
        return ticket.Position;
    }

    public async Task<bool> UpdateTicketCategory(Guid ticketId, Category taskCategory, CancellationToken token)
    {
        var ticket =  await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        ticket.Category = taskCategory;
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> DeleteTicketCategory(Guid ticketId, CancellationToken token)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        ticket.Category = null;
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> AddCategoryToTicket(Guid ticketId, Category taskCategory, CancellationToken token)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        ticket.Category = taskCategory;
        return true;
    }

    public async Task<bool> DeleteCategoryToTicket(Guid ticketId, CancellationToken token)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        ticket.Category = null;
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> AddTagToTicket(Guid ticketId, Tag taskTag, CancellationToken token)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        ticket.Tags.Add(taskTag);
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> DeleteTagFromTicket(Guid ticketId, string taskTag, CancellationToken token)
    {
        var ticket = await _context.Tickets.Include(ticket => ticket.Tags).FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        if (ticket != null)
        {
            var ticketTag= ticket.Tags.FirstOrDefault(x => x.TagName == taskTag);
            if (ticketTag != null) ticket.Tags.Remove(ticketTag);
            await _context.SaveChangesAsync(token);
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateTagToTicket(Guid ticketId, Tag taskTag, CancellationToken token)
    {
        var ticket = await _context.Tickets.Include(ticket => ticket.Tags).FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        var ticketTag= ticket.Tags.FirstOrDefault(x => x.TagName == taskTag.TagName);
        ticket.Tags.Remove(ticketTag);
        ticket.Tags.Add(taskTag);
        ticket.Tags.Add(taskTag);
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> AddSubTicketToTicket(SubTicket subTicket, CancellationToken token)
    {
        var ticket =  await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == subTicket.TicketId, token);
        ticket.SubTickets.Add(subTicket);
        return true;
    }

    public async Task<bool> DeleteSubTicketFromTicket(Guid subTicket, CancellationToken token)
    {
        var subTick = await _context.Subtickets.FirstOrDefaultAsync(x => x.TicketId == subTicket, token);
        if (subTick != null)
        {
            subTick.TicketId = Guid.Empty;
            _context.Remove(subTick);
            await _context.SaveChangesAsync(token);
            return true;
        }

        return false;


    }

    public async Task<bool> UpdateSubTicketToTicket(SubTicket subTicket, CancellationToken token)
    {
        var oldSubTicket = await _context.Subtickets.FirstOrDefaultAsync(x => x.SubTicketId == subTicket.SubTicketId, token);
        if (oldSubTicket != null)
        {
            oldSubTicket.Description = subTicket.Description;
            oldSubTicket.isComplete = subTicket.isComplete;
            oldSubTicket.TaskType = subTicket.TaskType;
            oldSubTicket.Title = subTicket.Title;
        }

        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<List<ICollection<Tag>>> GetAllTicketTags(Guid ticketId, CancellationToken token)
    {
        var tags = await _context.Tickets
            .Where(x => x.TicketId == ticketId)
            .Select(x => x.Tags).ToListAsync(token);

        return tags;
    }

    public async Task<IEnumerable<SubTicket>> GetAllTicketSubTickets(Guid ticketId, CancellationToken token)
    {
        var subTickets =  _context.Subtickets
            .Where(x => x.TicketId == ticketId).AsEnumerable();
        
        return subTickets;
    }

    public async Task<bool> ChangePriority(Guid ticketId, Priority taskPriority, CancellationToken token)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        ticket.Priority = taskPriority;
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> AddDeadline(DateTime deadline, Guid ticketId, CancellationToken token)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        ticket.EndDate = deadline;
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> DeleteDeadline( Guid ticketId, CancellationToken token)
    {
        var ticket =  await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        ticket.EndDate = null;
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<bool> UpdateDeadline(DateTime startDay, DateTime endDay, Guid ticketId, CancellationToken token)
    {
        var ticket =  await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId, token);
        if (ticket != null)
        {
            ticket.StartDate = startDay;
            ticket.EndDate = endDay;
            await _context.SaveChangesAsync(token);
            return true;
        }
        return false;
       
    }

    public async Task<bool> CompleteTicket(Guid ticketId, CancellationToken token)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId);
        ticket.isComplete = true;
        await _context.SaveChangesAsync(token);
        return true;
    }
}