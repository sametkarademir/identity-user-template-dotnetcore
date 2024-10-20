using MimeKit;

namespace Application.Dtos.Mails;

public class MailKitSendMailRequestDto
{
    public string Subject { get; set; }
    public string TextBody { get; set; }
    public string HtmlBody { get; set; }
    public MailboxAddress To { get; set; }
    public string? UnsubscribeLink { get; set; }

    public MailKitSendMailRequestDto()
    {
        Subject = string.Empty;
        TextBody = string.Empty;
        HtmlBody = string.Empty;
        To = new MailboxAddress(string.Empty, string.Empty);
    }

    public MailKitSendMailRequestDto(string subject, string textBody, string htmlBody, MailboxAddress to)
    {
        Subject = subject;
        TextBody = textBody;
        HtmlBody = htmlBody;
        To = to;
    }
}