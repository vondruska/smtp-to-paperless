namespace SmtpToPaperless
{
    public interface IPaperlessClient
    {
        Task UploadFileAsync(string fileName, Stream fileToUpload, CancellationToken cancellationToken);
    }
}

