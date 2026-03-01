namespace TaskService.DTO.Response;

public class ErrorResponse
{
    public string Error { get; set; } = string.Empty;
    public string? Details { get; set; }
    public string? StackTrace { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// Ответ с валидационными ошибками
public class ValidationErrorResponse : ErrorResponse
{
    public Dictionary<string, string[]> ValidationErrors { get; set; } = new();
}

// Успешный ответ с сообщением
public class SuccessResponse
{
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// Ответ с пагинацией
public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}