using OnlineStore.AuthService.Core.Abstractions;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using OnlineStore.AuthService.Models;

namespace OnlineStore.AuthService.Core.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool SendEmail(AuthUser user, string confirmationToken)
    {
        using (var client = new SmtpClient())
        {
            client.Host = _configuration["GmailClient:Host"];
            client.Port = int.Parse(_configuration["GmailClient:Port"]);
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(_configuration["GmailClient:Email"], _configuration["GmailClient:AppPassword"]);
            using (var message = new MailMessage(
                from: new MailAddress(_configuration["GmailClient:Email"], "OnlineStore"),
                to: new MailAddress(user.Email, user.UserName)
                ))
            {
                message.Subject = $"ConfirmationToken for {user.UserName}";
                message.Body = $"User this token to confirm your email - `{confirmationToken}`";

                client.Send(message);
            }
        }

        return true;
    }
}
