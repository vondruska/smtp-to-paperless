using System.Buffers;
using Microsoft.Extensions.Logging;
using SmtpServer;
using SmtpServer.Protocol;
using SmtpServer.Storage;

namespace SmtpToPaperless
{
    public class PaperlessMessageStore : MessageStore
    {
        private readonly IMessageHandler _messageHandler;
        private readonly ILogger<PaperlessMessageStore> _logger;
        public PaperlessMessageStore(IMessageHandler handler, ILogger<PaperlessMessageStore> logger)
        {
            _logger = logger;
            _messageHandler = handler;
        }
        public async override Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction, ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Got message from {0}", $"{transaction.From.User}@{transaction.From.Host}");
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

                await _messageHandler.HandleMessageAsync(message, cancellationToken);

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