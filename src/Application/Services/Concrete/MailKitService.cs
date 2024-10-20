using Application.Dtos.Mails;
using Application.Services.Interface;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;

namespace Application.Services.Concrete;

public class MailKitService(IConfiguration configuration) : IMailKitService
{
    public void SendMail(MailKitSendMailRequestDto mail)
    {
        if (string.IsNullOrEmpty(mail.To.Address))
            return;
        
        EmailPrepare(mail, email: out MimeMessage email, smtp: out SmtpClient smtp);
        smtp.Send(email);
        smtp.Disconnect(true);
        email.Dispose();
        smtp.Dispose();
    }
    public async Task SendEmailAsync(MailKitSendMailRequestDto mail)
    {
        if (string.IsNullOrEmpty(mail.To.Address))
            return;
        
        EmailPrepare(mail, email: out MimeMessage email, smtp: out SmtpClient smtp);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
        email.Dispose();
        smtp.Dispose();
    }
    private void EmailPrepare(MailKitSendMailRequestDto mail, out MimeMessage email, out SmtpClient smtp)
    {
        email = new MimeMessage();
        email.From.Add(new MailboxAddress(configuration["Email:Configuration:SenderName"], configuration["Email:Configuration:SenderEmail"]));
        email.To.Add(mail.To);
        email.Subject = mail.Subject;
        if (mail.UnsubscribeLink != null)
            email.Headers.Add(field: "List-Unsubscribe", value: $"<{mail.UnsubscribeLink}>");
        BodyBuilder bodyBuilder = new() { TextBody = mail.TextBody, HtmlBody = mail.HtmlBody };

        email.Body = bodyBuilder.ToMessageBody();
        email.Prepare(EncodingConstraint.SevenBit);

        if (configuration["Email:Configuration:DkimPrivateKey"] != null && configuration["Email:Configuration:DkimSelector"] != null && configuration["Email:Configuration:DomainName"] != null)
        {
            DkimSigner signer =
                new(key: ReadPrivateKeyFromPemEncodedString(), configuration["Email:Configuration:DomainName"], configuration["Email:Configuration:DkimSelector"])
                {
                    HeaderCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                    BodyCanonicalizationAlgorithm = DkimCanonicalizationAlgorithm.Simple,
                    AgentOrUserIdentifier = $"@{configuration["Email:Configuration:DomainName"]}",
                    QueryMethod = "dns/txt"
                };
            HeaderId[] headers = { HeaderId.From, HeaderId.Subject, HeaderId.To };
            signer.Sign(email, headers);
        }

        smtp = new SmtpClient();
        smtp.Connect(configuration["Email:Configuration:Server"], Convert.ToInt16(configuration["Email:Configuration:Port"]));
        if (Convert.ToBoolean(configuration["Email:Configuration:AuthenticationRequired"]))
            smtp.Authenticate(configuration["Email:Configuration:UserName"], configuration["Email:Configuration:Password"]);
    }
    private AsymmetricKeyParameter ReadPrivateKeyFromPemEncodedString()
    {
        AsymmetricKeyParameter result;
        string pemEncodedKey =
            "-----BEGIN RSA PRIVATE KEY-----\n" + configuration["Email:Configuration:DkimPrivateKey"] + "\n-----END RSA PRIVATE KEY-----";
        using (StringReader stringReader = new(pemEncodedKey))
        {
            PemReader pemReader = new(stringReader);
            object? pemObject = pemReader.ReadObject();
            result = ((AsymmetricCipherKeyPair)pemObject).Private;
        }

        return result;
    }
}