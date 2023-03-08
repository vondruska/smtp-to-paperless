using Microsoft.Extensions.Hosting;

namespace SmtpToPaperless
{
    public sealed class Worker : BackgroundService
    {
        readonly SmtpServer.SmtpServer _smtpServer;

        public Worker(SmtpServer.SmtpServer smtpServer)
        {
            _smtpServer = smtpServer;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return _smtpServer.StartAsync(stoppingToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _smtpServer.Shutdown();

            return base.StopAsync(cancellationToken);
        }
    }
}