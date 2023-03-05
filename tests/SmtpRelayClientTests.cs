using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;

namespace SmtpToPaperless.Tests;

public class SmtpRelayClientTests
{
    const string ReplacedFromAddress = "from-must-be-this@example.com";
    [Fact]
    public async Task From_Address_Is_Replaced_When_Configured()
    {
        var options = Options.Create(new Configuration
        {
            RelayFrom = ReplacedFromAddress
        });

        var message = Factory.DummyMessageWithAttachment("random-address@example.com", "to@example.com");

        var smtpClient = Factory.SmtpClient;

        var smtpRelayClient = new SmtpRelayClient(smtpClient, options);
        await smtpRelayClient.RelayMessageAsync(message, CancellationToken.None);

        message.From.First().ShouldBe(MailboxAddress.Parse(ReplacedFromAddress));
    }
}