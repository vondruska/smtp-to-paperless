using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace SmtpToPaperless.Tests;

public class MessageHandlerTests
{
    const string NoRelayTo = "no-relay@example.com";
    const string ShouldRelayTo = "relay-to-this@example.com";
    const string FromAddress = "from@example.com";

    [Fact]
    public async Task Does_Not_Relay_When_Not_Configured()
    {
        var message = Factory.DummyMessageWithAttachment(FromAddress, NoRelayTo);
        var options = Options.Create(new Configuration());

        var smtpRelay = Substitute.For<ISmtpRelayClient>();
        smtpRelay.RelayMessageAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        var paperlessClient = Substitute.For<IPaperlessClient>();
        paperlessClient.UploadFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        var handler = new MessageHandler(options, smtpRelay, paperlessClient, NullLogger<MessageHandler>.Instance);
        await handler.HandleMessageAsync(message, CancellationToken.None);

        await paperlessClient.Received(1).UploadFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>());
        await smtpRelay.DidNotReceiveWithAnyArgs().RelayMessageAsync(default, default);
    }

    [Fact]
    public async Task Does_Relay_When_Email_Matched()
    {
        var message = Factory.DummyMessageWithAttachment(FromAddress, ShouldRelayTo);
        var options = Options.Create(new Configuration
        {
            RelayFor = ShouldRelayTo,
            RelayHost = "smtp.example.com"
        });

        var smtpRelay = Substitute.For<ISmtpRelayClient>();
        smtpRelay.RelayMessageAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        var paperlessClient = Substitute.For<IPaperlessClient>();
        paperlessClient.UploadFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        var handler = new MessageHandler(options, smtpRelay, paperlessClient, NullLogger<MessageHandler>.Instance);
        await handler.HandleMessageAsync(message, CancellationToken.None);

        await smtpRelay.ReceivedWithAnyArgs(1).RelayMessageAsync(default, default);
        await paperlessClient.DidNotReceiveWithAnyArgs().UploadFileAsync(default, default, default);
    }

    [Fact]
    public async Task Does_Not_Relay_When_No_Match_While_Configured()
    {
        var message = Factory.DummyMessageWithAttachment(FromAddress, NoRelayTo);
        var options = Options.Create(new Configuration
        {
            RelayFor = ShouldRelayTo,
            RelayHost = "smtp.example.com"
        });

        var smtpRelay = Substitute.For<ISmtpRelayClient>();
        smtpRelay.RelayMessageAsync(Arg.Any<MimeMessage>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        var paperlessClient = Substitute.For<IPaperlessClient>();
        paperlessClient.UploadFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        var handler = new MessageHandler(options, smtpRelay, paperlessClient, NullLogger<MessageHandler>.Instance);
        await handler.HandleMessageAsync(message, CancellationToken.None);

        await paperlessClient.Received(1).UploadFileAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>());
        await smtpRelay.DidNotReceiveWithAnyArgs().RelayMessageAsync(default, default);
    }
}