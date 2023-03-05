using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace SmtpToPaperless;

public class MessageHandler : IMessageHandler
{
    private readonly Configuration _configuration;
    private readonly ISmtpRelayClient _smtpRelay;
    private readonly IPaperlessClient _paperlessClient;
    private readonly ILogger<MessageHandler> _logger;
    public MessageHandler(IOptions<Configuration> configuration, ISmtpRelayClient smtpRelay, IPaperlessClient paperlessClient, ILogger<MessageHandler> logger)
    {
        _configuration = configuration.Value;
        _smtpRelay = smtpRelay;
        _paperlessClient = paperlessClient;
        _logger = logger;
    }
    public async Task HandleMessageAsync(MimeMessage message, CancellationToken cancellationToken)
    {
        if (ShouldMessageBeRelayed(message))
        {
            await RelayMessageAsync(message, cancellationToken);
        }
        else
        {
            await UploadAttachmentsAsync(message, cancellationToken);
        }
    }

    private async Task UploadAttachmentsAsync(MimeKit.MimeMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Message has {0} attachments", message.Attachments.Count());

        foreach (var attachment in message.Attachments)
        {
            using var attachmentStream = new MemoryStream();
            if (attachment is MimePart)
                ((MimePart)attachment).Content.DecodeTo(attachmentStream);
            else
                ((MessagePart)attachment).Message.WriteTo(attachmentStream);

            await _paperlessClient.UploadFileAsync(attachment.ContentDisposition.FileName, attachmentStream, cancellationToken);
        }
    }

    private async Task RelayMessageAsync(MimeKit.MimeMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Relaying message");
        await _smtpRelay.RelayMessageAsync(message, cancellationToken);
    }

    private bool ShouldMessageBeRelayed(MimeKit.MimeMessage message)
    {
        // not configured
        if (String.IsNullOrEmpty(_configuration.RelayHost) || String.IsNullOrEmpty(_configuration.RelayFor))
        {
            return false;
        }

        // I really don't want to deal with split brain
        // one message goes to multiple systems
        // YGNI
        if (message.To.Count() != 1)
        {
            return false;
        }


        var toAddress = message.To.First() as MailboxAddress;
        if (toAddress != null && _configuration.RelayForSplit.Contains(toAddress.Address))
        {
            return true;
        }
        return false;
    }
}