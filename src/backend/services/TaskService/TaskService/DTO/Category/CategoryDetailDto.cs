using TaskService.DTO.Tag;
using TaskService.DTO.Ticket;

namespace TaskService.DTO.Category;

public class CategoryDetailDto : CategoryDto
{
    public List<TagDto> PopularTags { get; set; } = new();
    public Dictionary<string, int> TasksByPriority { get; set; } = new();
    public List<TicketSummaryDto> RecentTasks { get; set; } = new();
}