
using IdentityService.API.DTOs;
using IdentityService.API.Extensions;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace IdentityService.API.Services;

public sealed class EmailService(ISendGridClient sendGridClient, IOptions<EmailSettings> emailSettings)
                  : IEmailService
{
    private readonly ISendGridClient _sendGridClient = sendGridClient;
    private readonly EmailSettings _emailSettings = emailSettings.Value;

    public async Task SendAsync(EmailMessageDTO email)
    {
        var sendGridMessage = new SendGridMessage
        {
            From = new EmailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
            Subject = email.Subject
        };

        sendGridMessage.AddContent(MimeType.Text, email.Content);
        sendGridMessage.AddTo(new EmailAddress(email.To));
        await _sendGridClient.SendEmailAsync(sendGridMessage);
    }
}