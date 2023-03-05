using MailKit.Net.Smtp;
using MailKit.Security;

public static class Factory
{
    public static byte[] TextFile = System.Text.Encoding.UTF8.GetBytes("The quick brown fox jumped over the lazy dog");
    public static Stream TextFileAsStream => new MemoryStream(TextFile);
    public static MimeMessage DummyMessageWithAttachment(string from, string to)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("", from));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = "Test Subject";

        var builder = new BodyBuilder();

        // Set the plain-text version of the message text
        builder.TextBody = @"Test text body";

        var attachment = new MimePart("text", "plain")
        {
            Content = new MimeContent(TextFileAsStream),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = "test.txt"
        };

        // We may also want to attach a calendar event for Monica's party...
        builder.Attachments.Add(attachment);
        // Now we just need to set the message body and we're done
        message.Body = builder.ToMessageBody();

        return message;

    }

    public static ISmtpClient SmtpClient
    {
        get
        {
            var smtpClient = Substitute.For<ISmtpClient>();
            smtpClient.ConnectAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<SecureSocketOptions>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            smtpClient.AuthenticateAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
            smtpClient.SendAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult("hello world"));
            smtpClient.DisconnectAsync(Arg.Any<bool>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

            return smtpClient;
        }
    }
}