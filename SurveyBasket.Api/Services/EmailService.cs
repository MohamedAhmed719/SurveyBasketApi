using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using SurveyBasket.Api.Settings;

namespace SurveyBasket.Api.Services;

public class EmailService(IOptions<MailSettings> mailSettings) : IEmailSender
{
    private readonly MailSettings _mailSettings = mailSettings.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_mailSettings.Mail),
            Subject = subject
        };

        message.To.Add(MailboxAddress.Parse(email));

        var body = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        message.Body = body.ToMessageBody();

        using var smtp = new SmtpClient();

        smtp.Connect(_mailSettings.Host, _mailSettings.Port,SecureSocketOptions.StartTls);

        smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

        await smtp.SendAsync(message);

        await smtp.DisconnectAsync(true);
       
    }
}
