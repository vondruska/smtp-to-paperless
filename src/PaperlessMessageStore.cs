using System.Buffers;
using Microsoft.Extensions.Logging;
using MimeKit;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace SmtpToPaperless
{
    public class PaperlessMessageStore : MessageStore
    {
        private readonly IPaperlessClient _paperlessClient;
        private readonly ILogger<PaperlessMessageStore> _logger;
        public PaperlessMessageStore(IPaperlessClient paperlessClient, ILogger<PaperlessMessageStore> logger)
        {
            _paperlessClient = paperlessClient ?? throw new ArgumentNullException(nameof(paperlessClient));
            _logger = logger;
        }
        public async override Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
        {
            try
            {
                await using var stream = new MemoryStream();

                var position = buffer.GetPosition(0);
                while (buffer.TryGet(ref position, out var memory))
                {
                    await stream.WriteAsync(memory, cancellationToken);
                }

                stream.Position = 0;

                var message = await MimeKit.MimeMessage.LoadAsync(stream, cancellationToken);

                foreach (var attachment in message.Attachments)
                {
                    using var attachmentStream = new MemoryStream();
                    if (attachment is MimePart)
                        ((MimePart)attachment).Content.DecodeTo(attachmentStream);
                    else
                        ((MessagePart)attachment).Message.WriteTo(attachmentStream);

                    await _paperlessClient.UploadFileAsync(attachment.ContentDisposition.FileName, attachmentStream, cancellationToken);
                }
                return SmtpResponse.Ok;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process incoming message");
                return SmtpResponse.TransactionFailed;
            }

        }
    }
}