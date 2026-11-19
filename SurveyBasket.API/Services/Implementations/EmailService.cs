using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using SurveyBasket.API.Settings;

namespace SurveyBasket.API.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public EmailService(IOptions<EmailSettings> options)
    {
        _emailSettings = options.Value;
    }

    public async Task SendAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();

        email.From.Add(new MailboxAddress(_emailSettings.DisplayName, _emailSettings.Username));

        email.To.Add(MailboxAddress.Parse(to));

        email.Subject = subject;

        email.Body = new BodyBuilder
        {
            HtmlBody = body
        }.ToMessageBody();

        using var smtpClient = new SmtpClient();

        await smtpClient.ConnectAsync(
            _emailSettings.Host,
            _emailSettings.Port,
            SecureSocketOptions.StartTls,
            cancellationToken);

        await smtpClient.AuthenticateAsync(
            _emailSettings.Username,
            _emailSettings.Password,
            cancellationToken);

        await smtpClient.SendAsync(email, cancellationToken: cancellationToken);

        await smtpClient.DisconnectAsync(true, cancellationToken);
    }

}
