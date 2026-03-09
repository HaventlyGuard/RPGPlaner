using RabbitMQ.Client;

namespace TaskService.EventHandlers;

public static class RabbitMqConnection
{
    private static readonly ConnectionFactory factory = new ConnectionFactory 
    { 
        HostName = "localhost",
        UserName = "guest",
        Password = "guest",
        Port = 5672,
        
        AutomaticRecoveryEnabled = true,
        NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
        RequestedHeartbeat = TimeSpan.FromSeconds(60)
    };
    
    private static IConnection? _connection;
    private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
    
    public static async Task<IConnection> GetConnectionAsync()
    {
        if (_connection != null && _connection.IsOpen)
            return _connection;
            
        await _semaphore.WaitAsync();
        try
        {
            if (_connection != null && _connection.IsOpen)
                return _connection;
                
            _connection = await factory.CreateConnectionAsync();
            Console.WriteLine("RabbitMQ connection established");
            
            _connection.ConnectionShutdownAsync += (sender, e) =>
            {
                Console.WriteLine("RabbitMQ connection shutdown");
                return Task.CompletedTask;
            };
            
            return _connection;
        }
        finally
        {
            _semaphore.Release();
        }
    }
    
    public static async Task<IChannel> CreateChannelAsync()
    {
        var connection = await GetConnectionAsync();
        return await connection.CreateChannelAsync();
    }
}