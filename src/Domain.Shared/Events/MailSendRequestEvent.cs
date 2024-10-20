using EventBus.Base.Events;

namespace Domain.Shared.Events;

public class MailSendRequestEvent : IntegrationEvent
{
    public MailSendRequestEvent()
    {
        Name = string.Empty;
        To = string.Empty;
        Subject = string.Empty;
        Body = string.Empty;
    }
    public MailSendRequestEvent(string to, string subject, string body, string name)
    {
        To = to;
        Subject = subject;
        Body = body;
        Name = name;
    }

    public MailSendRequestEvent(DateTime createdDate, Guid eventId, string to, string subject, string body, string name) : base(createdDate, eventId)
    {
        To = to;
        Subject = subject;
        Body = body;
        Name = name;
    }

    public string Name { get; set; }
    public string To { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}