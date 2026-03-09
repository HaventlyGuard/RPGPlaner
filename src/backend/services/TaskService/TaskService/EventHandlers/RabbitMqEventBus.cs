using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TaskService.EventHandlers.Contracts;

namespace TaskService.EventHandlers;

public class RabbitMqEventBus :  IAsyncDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _exchangeName = "todo_app_exchange";
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<RabbitMqEventBus> _logger;
        private readonly Dictionary<string, Type> _eventTypes;
        private readonly Dictionary<string, List<Type>> _handlers;
        
        public RabbitMqEventBus(IServiceProvider serviceProvider, ILogger<RabbitMqEventBus> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _eventTypes = new Dictionary<string, Type>();
            _handlers = new Dictionary<string, List<Type>>();
            
            _connection = RabbitMqConnection.GetConnectionAsync().GetAwaiter().GetResult();
            _channel = RabbitMqConnection.CreateChannelAsync().GetAwaiter().GetResult();
            
            _channel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: "topic",
                durable: true,
                autoDelete: false
            ).GetAwaiter().GetResult();
        }
        
        public async Task PublishAsync<T>(T @event) where T : IIntegrationEvent
        {
            var eventName = typeof(T).Name;
            var routingKey = eventName.ToLowerInvariant();
            
            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);
            
            var properties = new BasicProperties
            {
                Persistent = true,
                Type = eventName,
                MessageId = @event.Id.ToString(),
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };
            
            await _channel.BasicPublishAsync(
                exchange: _exchangeName,
                routingKey: routingKey,
                mandatory: true,
                basicProperties: properties,
                body: body
            );
            
            _logger.LogInformation("Published event {EventName} with Id {EventId}", eventName, @event.Id);
        }
        
        public async Task SubscribeAsync<T, TH>()
            where T : IIntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);
            
            if (!_handlers.ContainsKey(eventName))
            {
                _handlers[eventName] = new List<Type>();
            }
            
            _handlers[eventName].Add(handlerType);
            _eventTypes[eventName] = typeof(T);
            
            var queueName = $"{handlerType.Name}_queue";
            
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );
            
            await _channel.QueueBindAsync(
                queue: queueName,
                exchange: _exchangeName,
                routingKey: eventName.ToLowerInvariant()
            );
            
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                await HandleMessage(ea);
            };
            
            await _channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: false,
                consumer: consumer
            );
            
            _logger.LogInformation("Subscribed {Handler} to {EventName}", handlerType.Name, eventName);
        }
        
        private async Task HandleMessage(BasicDeliverEventArgs ea)
        {
            var eventName = ea.BasicProperties.Type;
            var message = Encoding.UTF8.GetString(ea.Body.ToArray());
            
            try
            {
                if (_eventTypes.TryGetValue(eventName, out var eventType))
                {
                    var @event = JsonSerializer.Deserialize(message, eventType) as IIntegrationEvent;
                    
                    if (_handlers.TryGetValue(eventName, out var handlerTypes))
                    {
                        using var scope = _serviceProvider.CreateScope();
                        
                        foreach (var handlerType in handlerTypes)
                        {
                            var handler = scope.ServiceProvider.GetService(handlerType);
                            if (handler != null)
                            {
                                var method = handlerType.GetMethod("Handle");
                                await (Task)method.Invoke(handler, new[] { @event });
                            }
                        }
                    }
                }
                
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {EventName}", eventName);
                await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
            }
        }
        
        public async ValueTask DisposeAsync()
        {
            if (_channel != null && _channel.IsOpen)
            {
                await _channel.CloseAsync();
                await _channel.DisposeAsync();
            }
        }
    }