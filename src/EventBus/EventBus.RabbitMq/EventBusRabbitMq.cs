using System.Net.Sockets;
using System.Text;
using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMq;

public class EventBusRabbitMq : BaseEventBus
{
    private RabbitMqPersistentConnection _persistentConnection;
    private readonly IConnectionFactory? connectionFactory;
    private readonly IModel consumerChannel;
    
    
    public EventBusRabbitMq(EventBusConfig config, IServiceProvider serviceProvider) : base(config, serviceProvider)
    {
        if (config.Connection != null)
        {
            var connJson = Newtonsoft.Json.JsonConvert.SerializeObject(EventBusConfig.Connection, new Newtonsoft.Json.JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
            });

            connectionFactory = Newtonsoft.Json.JsonConvert.DeserializeObject<ConnectionFactory>(connJson, new Newtonsoft.Json.JsonSerializerSettings
            {
                TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
            });
        }
        else
        {
            connectionFactory = new ConnectionFactory();
        }
        _persistentConnection = new RabbitMqPersistentConnection(connectionFactory, config.ConnectionRetryCount);
        
        consumerChannel = CreateConsumerChannel();
        SubsManager.OnEventRemoved += SubsManagerOnOnEventRemoved;
    }

    private void SubsManagerOnOnEventRemoved(object? sender, string eventName)
    {
        eventName = ProcessEventName(eventName);
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }
        
        consumerChannel.QueueUnbind(queue: eventName, exchange: EventBusConfig.DefaultTopicName, routingKey: eventName);

        if (SubsManager.IsEmpty)
        {
            consumerChannel.Close();
        }
    }

    private IModel CreateConsumerChannel()
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var channel = _persistentConnection.CreateModel();
        channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");
        return channel;
    }

    private void StartBaseConsume(string eventName)
    {
        if (consumerChannel != null)
        {
            var consumer = new EventingBasicConsumer(consumerChannel);
            consumer.Received += ConsumerOnReceived;

            consumerChannel.BasicConsume(
                queue: GetSubName(eventName),
                autoAck: false,
                consumer: consumer
            );
        }
    }
    private async void ConsumerOnReceived(object? sender, BasicDeliverEventArgs e)
    {
        var eventName = e.RoutingKey;
        eventName = ProcessEventName(eventName);

        var message = Encoding.UTF8.GetString(e.Body.Span);

        try
        {
            await ProcessEvent(eventName, message);
        }
        catch (Exception exception)
        {
            //Log
        }
        
        consumerChannel.BasicAck(e.DeliveryTag, multiple: false);
    }


    public override void Publish(IntegrationEvent @event)
    {
        if (!_persistentConnection.IsConnected)
        {
            _persistentConnection.TryConnect();
        }

        var policy = Policy.Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(EventBusConfig.ConnectionRetryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) =>
                {
                    //Log
                });

        var eventName = @event.GetType().Name;
        eventName = ProcessEventName(eventName);
        
        consumerChannel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");

        var message = JsonConvert.SerializeObject(@event);
        var body = Encoding.UTF8.GetBytes(message);

        policy.Execute(() =>
        {
            var properties = consumerChannel.CreateBasicProperties();
            properties.DeliveryMode = 2;

            // consumerChannel.QueueDeclare(queue: GetSubName(eventName), durable: true, exclusive: false, autoDelete: false, arguments: null);
            //
            // consumerChannel.QueueBind(
            //     queue: GetSubName(eventName),
            //     exchange: EventBusConfig.DefaultTopicName,
            //     routingKey: eventName
            // );
            
            consumerChannel.BasicPublish(exchange: EventBusConfig.DefaultTopicName, routingKey: eventName,
                mandatory: true, basicProperties: properties, body: body);
        });
    }

    public override void Subscribe<T, TH>()
    {
        var eventName = typeof(T).Name;
        eventName = ProcessEventName(eventName);

        if (!SubsManager.HasSubscriptionForEvent(eventName))
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            consumerChannel.QueueDeclare(
                queue: GetSubName(eventName),
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
            
            consumerChannel.QueueBind(
                queue: GetSubName(eventName),
                exchange: EventBusConfig.DefaultTopicName,
                routingKey: eventName
            );
        }
        
        SubsManager.AddSubscription<T, TH>();
        StartBaseConsume(eventName);
    }
    public override void UnSubscribe<T, TH>()
    {
        SubsManager.RemoveSubscription<T, TH>();
    }
}