namespace EventBus.Base;

public class EventBusConfig
{
    public int ConnectionRetryCount { get; set; } = 5;
    public string DefaultTopicName { get; set; } = "DefaultTopicName";
    public string EventBusConnectionString { get; set; } = String.Empty;
    public string SubscriberClientAppName { get; set; } = String.Empty;
    public string EventNamePrefix { get; set; } = string.Empty;
    public string EventNameSuffix { get; set; } = "IntegrationEvent";
    public EventBusType EventBusType { get; set; } = EventBusType.RabbitMq;
    public object? Connection { get; set; }

    public bool DeleteEventPrefix => !string.IsNullOrEmpty(EventNamePrefix);
    public bool DeleteEventSuffix => !string.IsNullOrEmpty(EventNameSuffix);

}

public enum EventBusType
{
    RabbitMq = 0,
    AzureServiceBus = 1
}