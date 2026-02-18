namespace TaskService.Errors.Exceptions;

public class AppException : Exception
{
    int StatusCode { get; set; }
    public AppException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public class ColumnNotFoundExcepion : AppException
{
    public ColumnNotFoundExcepion() : base("Column was not found", 405){}
    
}

public class TaskNotFoundExcepion : AppException
{
    public TaskNotFoundExcepion() : base("Task was not found", 406){}
}

public class TaskCreateExcepion : AppException
{
    public TaskCreateExcepion() : base("Task can't be created", 407){}
}

public class DataBaseExcepion : AppException
{
    public DataBaseExcepion() : base("Database isn't connected", 410){}
}

public class ValidationExcepion : AppException
{
    public ValidationExcepion() : base("Data is not valid", 407){}
    
}

