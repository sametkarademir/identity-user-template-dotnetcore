using EventBus.AzureServiceBus;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.RabbitMq;

namespace EventBus.Factory;

public static class EventBusFactory
{
    public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
    {
        return config.EventBusType switch
        {
            EventBusType.AzureServiceBus => new EventBusServiceBus(config, serviceProvider),
            EventBusType.RabbitMq => new EventBusRabbitMq(config, serviceProvider),
            _ => new EventBusRabbitMq(config, serviceProvider),
        };
    }
}