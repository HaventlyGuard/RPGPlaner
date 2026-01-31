using Microsoft.EntityFrameworkCore;
using TaskService.DataAcces;
using TaskService.Models;
using TaskService.Repositories.Interfaces;

namespace TaskService.Repositories;

public class TagRepository : ITagRepository
{
    private readonly TaskContext _context;
    public TagRepository(TaskContext context)
    {
        _context = context;
    }
    public async Task<Tag?> GetTag(string tagName, CancellationToken token)
    {
        var tag = _context.Tags.FirstOrDefault(x => x.TagName == tagName);
        return tag;
    }

    public async Task<bool> AddTag(Tag tag, CancellationToken token)
    {
        var newTag = new Tag
        {
            TagColor = tag.TagColor,
            TagName = tag.TagName,
            Category = tag.Category,
        };
        await _context.Tags.AddAsync(newTag, token);
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<Tag> UpdateTag(Tag tag, CancellationToken token)
    {
        var tagToUpdate = _context.Tags
            .FirstOrDefault(x => x.TagName == tag.TagName);
        tagToUpdate.TagColor = tag.TagColor;
        tagToUpdate.TagName = tag.TagName;
        tagToUpdate.Category = tag.Category;
        await _context.SaveChangesAsync(token);
        return tagToUpdate;
    }

    public async Task<bool> DeleteTag(string tagId, CancellationToken token)
    {
        var tagToDelete = _context.Tags
            .FirstOrDefault(x => x.TagName == tagId);
        if (tagToDelete != null) _context.Tags
            .Remove(tagToDelete);
        await _context.SaveChangesAsync(token);
        return true;
    }

    public async Task<IEnumerable<Tag>> GetTags(CancellationToken token)
    {
        var tags =  await _context.Tags
            .ToListAsync(token);
        return tags;
    }

    public async Task<IEnumerable<Tag>> GetAllTicketTags(Guid ticketId, CancellationToken token)
    {
        var ticket = await _context.Tickets
            .Include(ticket => ticket.Tags)
            .FirstOrDefaultAsync(x => x.TicketId == ticketId, cancellationToken: token);
        if (ticket != null) return ticket.Tags;
        return [];
    }
}