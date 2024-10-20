using Newtonsoft.Json;

namespace EventBus.Base.Events;

public class IntegrationEvent 
{
    [JsonProperty]
    public Guid EventId { get; private set; }
    [JsonProperty]
    public DateTime CreatedDate { get; private set; }
    
    
    public IntegrationEvent()
    {
        EventId = Guid.NewGuid();
        CreatedDate = DateTime.Now;
    }
    
    [System.Text.Json.Serialization.JsonConstructor]
    public IntegrationEvent(DateTime createdDate, Guid eventId)
    {
        CreatedDate = createdDate;
        EventId = eventId;
    }
}