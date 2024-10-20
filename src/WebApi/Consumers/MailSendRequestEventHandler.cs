using Application.Dtos.Mails;
using Application.Services.Interface;
using Domain.Shared.Events;
using EventBus.Base.Abstraction;
using MimeKit;

namespace WebApi.Consumers;

public class MailSendRequestEventHandler : IIntegrationEventHandler<MailSendRequestEvent>
{
    private readonly ILogger<MailSendRequestEventHandler> _logger;
    private readonly IMailKitService _mailKitService;

    public MailSendRequestEventHandler(ILogger<MailSendRequestEventHandler> logger, IMailKitService mailKitService)
    {
        _logger = logger;
        _mailKitService = mailKitService;
    }

    public async Task Handle(MailSendRequestEvent @event)
    {
        _logger.LogInformation("Success MailSendRequestEventHandler");
        await _mailKitService.SendEmailAsync(new MailKitSendMailRequestDto
        {
            Subject = @event.Subject,
            HtmlBody = @event.Body,
            To = new MailboxAddress(@event.Name, @event.To)
        });
    }
}