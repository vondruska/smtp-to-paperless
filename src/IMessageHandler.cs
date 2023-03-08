using MimeKit;

namespace SmtpToPaperless;

public interface IMessageHandler
{
    Task HandleMessageAsync(MimeMessage message, CancellationToken cancellationToken);
}