using TaskService.Models;

namespace TaskService.Services.Interfaces;

public interface ITagService
{
    public Task<Tag> GetTag(string TagName, CancellationToken token);
    public Task<bool> AddTag(Tag tag, CancellationToken token);
    public Task<Tag> UpdateTag(Tag tag, CancellationToken token);
    public Task<bool> DeleteTag(string tag, CancellationToken token);
    
    public Task<IEnumerable<Tag>> GetTags( CancellationToken token);
    public Task<IEnumerable<Tag>> GetAllTicketTags(Guid ticketId, CancellationToken token);
}