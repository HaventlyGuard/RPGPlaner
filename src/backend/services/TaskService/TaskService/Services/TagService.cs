using Microsoft.Extensions.Logging;
using TaskService.Models;
using TaskService.Repositories.Interfaces;
using TaskService.Services.Interfaces;

namespace TaskService.Services;

public class TagService : ITagService
{
    private readonly ITagRepository _tagRepository;
    private readonly ILogger<TagService> _logger;
    private readonly ITicketRepository _ticketRepository; 

    public TagService(
        ITagRepository tagRepository,
        ILogger<TagService> logger,
        ITicketRepository ticketRepository)
    {
        _tagRepository = tagRepository ?? throw new ArgumentNullException(nameof(tagRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ticketRepository = ticketRepository ?? throw new ArgumentNullException(nameof(ticketRepository));
    }

    public async Task<Tag> GetTag(string tagName, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            _logger.LogWarning("Attempt to get tag with empty name");
            throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));
        }

        _logger.LogInformation("Getting tag: {TagName}", tagName);

        try
        {
            var tag = await _tagRepository.GetTag(tagName, token);
            
            if (tag == null)
            {
                _logger.LogWarning("Tag not found: {TagName}", tagName);
                throw new KeyNotFoundException($"Tag '{tagName}' not found");
            }

            return tag;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tag: {TagName}", tagName);
            throw;
        }
    }

    public async Task<bool> AddTag(Tag tag, CancellationToken token)
    {
        ValidateTag(tag);
        
        _logger.LogInformation("Adding new tag: {TagName}", tag.TagName);

        try
        {
            // Проверяем, не существует ли уже тег с таким именем
            var existingTag = await _tagRepository.GetTag(tag.TagName, token);
            if (existingTag != null)
            {
                _logger.LogWarning("Tag already exists: {TagName}", tag.TagName);
                throw new InvalidOperationException($"Tag '{tag.TagName}' already exists");
            }

            return await _tagRepository.AddTag(tag, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding tag: {TagName}", tag.TagName);
            throw;
        }
    }

    public async Task<Tag> UpdateTag(Tag tag, CancellationToken token)
    {
        ValidateTag(tag);
        
        _logger.LogInformation("Updating tag: {TagName}", tag.TagName);

        try
        {
            var existingTag = await _tagRepository.GetTag(tag.TagName, token);
            if (existingTag == null)
            {
                _logger.LogWarning("Tag not found for update: {TagName}", tag.TagName);
                throw new KeyNotFoundException($"Tag '{tag.TagName}' not found");
            }

            if (existingTag.TagName != tag.TagName)
            {
                var tagWithNewName = await _tagRepository.GetTag(tag.TagName, token);
                if (tagWithNewName != null)
                {
                    throw new InvalidOperationException($"Another tag already exists with name '{tag.TagName}'");
                }
            }

            return await _tagRepository.UpdateTag(tag, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tag: {TagName}", tag.TagName);
            throw;
        }
    }

    public async Task<bool> DeleteTag(string tagName, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));
        }

        _logger.LogInformation("Deleting tag: {TagName}", tagName);

        try
        {
            var existingTag = await _tagRepository.GetTag(tagName, token);
            if (existingTag == null)
            {
                _logger.LogWarning("Tag not found for deletion: {TagName}", tagName);
                return false; 
            }


            return await _tagRepository.DeleteTag(tagName, token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tag: {TagName}", tagName);
            throw;
        }
    }

    public async Task<IEnumerable<Tag>> GetTags(CancellationToken token)
    {
        _logger.LogInformation("Getting all tags");

        try
        {
            var tags = await _tagRepository.GetTags(token);
            
            if (!tags.Any())
            {
                _logger.LogDebug("No tags found in the system");
            }
            else
            {
                _logger.LogDebug("Retrieved {Count} tags", tags.Count());
            }

            return tags;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tags");
            throw;
        }
    }

    public async Task<IEnumerable<Tag>> GetAllTicketTags(Guid ticketId, CancellationToken token)
    {
        if (ticketId == Guid.Empty)
        {
            throw new ArgumentException("Ticket ID cannot be empty", nameof(ticketId));
        }

        _logger.LogInformation("Getting all tags for ticket: {TicketId}", ticketId);

        try
        {
            var ticket = await _ticketRepository.GetTicket(ticketId, token);
            if (ticket == null)
            {
                _logger.LogWarning("Ticket not found: {TicketId}", ticketId);
                throw new KeyNotFoundException($"Ticket '{ticketId}' not found");
            }

            var tags = await _tagRepository.GetAllTicketTags(ticketId, token);
            
            _logger.LogDebug("Retrieved {Count} tags for ticket {TicketId}", 
                tags.Count(), ticketId);

            return tags;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tags for ticket: {TicketId}", ticketId);
            throw;
        }
    }


    public async Task<IEnumerable<Tag>> SearchTags(string searchTerm, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetTags(token);
        }

        _logger.LogInformation("Searching tags with term: {SearchTerm}", searchTerm);

        try
        {
            var allTags = await _tagRepository.GetTags(token);
            var searchTermLower = searchTerm.ToLowerInvariant();
            
            return allTags.Where(t => 
                t.TagName.ToLowerInvariant().Contains(searchTermLower) ||
                (t.Category != null && t.Category.CompareTo(searchTermLower)==1));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tags with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<IEnumerable<Tag>> GetTagsByCategory(Category categoryId, CancellationToken token)
    {
        _logger.LogInformation("Getting tags by category: {CategoryId}", categoryId);

        try
        {
            var allTags = await _tagRepository.GetTags(token);
            return allTags.Where(t => t.Category== categoryId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tags by category: {CategoryId}", categoryId);
            throw;
        }
    }

    public async Task<bool> TagExists(string tagName, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(tagName))
        {
            return false;
        }

        try
        {
            var tag = await _tagRepository.GetTag(tagName, token);
            return tag != null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if tag exists: {TagName}", tagName);
            return false; // Или throw в зависимости от требований
        }
    }

    // Вспомогательные методы

    private void ValidateTag(Tag tag)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag));
        }

        if (string.IsNullOrWhiteSpace(tag.TagName))
        {
            throw new ArgumentException("Tag name cannot be null or empty", nameof(tag.TagName));
        }

        // Максимальная длина имени тега
        if (tag.TagName.Length > 50)
        {
            throw new ArgumentException("Tag name cannot exceed 50 characters", nameof(tag.TagName));
        }

        // Валидация цвета (если есть)
        if (!string.IsNullOrEmpty(tag.TagColor))
        {
            // Проверка HEX формата (#RRGGBB)
            if (!System.Text.RegularExpressions.Regex.IsMatch(tag.TagColor, "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"))
            {
                throw new ArgumentException("Tag color must be in valid HEX format (#RRGGBB or #RGB)", 
                    nameof(tag.TagColor));
            }
        }
    }

    // Метод для массового добавления тегов
    public async Task<IEnumerable<Tag>> AddTagsBatch(IEnumerable<Tag> tags, CancellationToken token)
    {
        if (tags == null || !tags.Any())
        {
            throw new ArgumentException("Tags collection cannot be null or empty", nameof(tags));
        }

        _logger.LogInformation("Adding batch of {Count} tags", tags.Count());

        var addedTags = new List<Tag>();
        var errors = new List<string>();

        foreach (var tag in tags)
        {
            try
            {
                ValidateTag(tag);
                
                // Пропускаем если уже существует
                var existingTag = await _tagRepository.GetTag(tag.TagName, token);
                if (existingTag != null)
                {
                    _logger.LogWarning("Tag already exists, skipping: {TagName}", tag.TagName);
                    errors.Add($"Tag '{tag.TagName}' already exists");
                    continue;
                }

                await _tagRepository.AddTag(tag, token);
                addedTags.Add(tag);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding tag in batch: {TagName}", tag.TagName);
                errors.Add($"Failed to add tag '{tag.TagName}': {ex.Message}");
            }
        }

        if (errors.Any())
        {
            _logger.LogWarning("Batch operation completed with {ErrorCount} errors", errors.Count);
            // Можно выбросить AggregateException или вернуть результат с ошибками
            // throw new AggregateException(errors.Select(e => new Exception(e)));
        }

        return addedTags;
    }
}