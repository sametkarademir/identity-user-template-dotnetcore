using Application.Dtos.Mails;

namespace Application.Services.Interface;

public interface IMailKitService
{
    void SendMail(MailKitSendMailRequestDto mail);
    Task SendEmailAsync(MailKitSendMailRequestDto mail);
}