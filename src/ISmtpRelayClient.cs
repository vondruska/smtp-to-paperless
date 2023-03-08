namespace SmtpToPaperless
{


    public interface ISmtpRelayClient
    {
        Task RelayMessageAsync(MimeKit.MimeMessage message, CancellationToken cancellationToken);
    }
}