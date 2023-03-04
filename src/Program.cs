// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmtpServer;
using SmtpServer.Storage;

namespace SmtpToPaperless
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        services.AddTransient<IMessageStore, PaperlessMessageStore>();
                        services.AddSingleton<IPaperlessClient, PaperlessClient>();

                        services.AddHttpClient();

                        services.AddSingleton(
                            provider =>
                            {
                                var options = new SmtpServerOptionsBuilder()
                                    .ServerName("SMTP Server")
                                    .Port(25)
                                    .Build();

                                return new SmtpServer.SmtpServer(options, provider.GetRequiredService<IServiceProvider>());
                            });

                        services.AddHostedService<Worker>();
                    });
    }
}