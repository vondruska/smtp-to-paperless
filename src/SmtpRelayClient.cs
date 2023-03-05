using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

namespace SmtpToPaperless
{
    public class SmtpRelayClient : ISmtpRelayClient
    {
        private readonly Configuration _configuration;
        private readonly ISmtpClient _smtpClient;
        public SmtpRelayClient(ISmtpClient smtpClient, IOptions<Configuration> configuration)
        {
            _configuration = configuration.Value;
            _smtpClient = smtpClient;
        }
        public async Task RelayMessageAsync(MimeKit.MimeMessage message, CancellationToken cancellationToken)
        {
            if (!String.IsNullOrEmpty(_configuration.RelayFrom))
            {
                message.From.Clear();
                message.From.Add(MailboxAddress.Parse(_configuration.RelayFrom));
            }

            await _smtpClient.ConnectAsync(_configuration.RelayHost, _configuration.RelayPort ?? 587, SecureSocketOptions.Auto, cancellationToken);
            await _smtpClient.AuthenticateAsync(_configuration.RelayUsername, _configuration.RelayPassword, cancellationToken);
            await _smtpClient.SendAsync(message, cancellationToken);
            await _smtpClient.DisconnectAsync(true, cancellationToken);

        }
    }
}