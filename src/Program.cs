// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
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
                .ConfigureHostConfiguration(hostConfig =>
                {
                    hostConfig.SetBasePath(Directory.GetCurrentDirectory());
                    hostConfig.AddJsonFile("settings.json", optional: true);
                    hostConfig.AddEnvironmentVariables(prefix: "App_");
                    hostConfig.AddCommandLine(args);
                })
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        services.Configure<Configuration>(hostContext.Configuration.GetSection(""));
                        services.AddTransient<IMessageStore, PaperlessMessageStore>();
                        services.AddSingleton<IPaperlessClient, PaperlessClient>();

                        services.AddHttpClient("Paperless", configure =>
                        {
                            configure.BaseAddress = new Uri(hostContext.Configuration["PaperlessBaseUrl"]);

                            var username = hostContext.Configuration["PaperlessUsername"];
                            var password = hostContext.Configuration["PaperlessPassword"];

                            if (!String.IsNullOrEmpty(username))
                            {
                                configure.DefaultRequestHeaders.Authorization =
                                    new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
                            }
                        });

                        services.AddSingleton(
                            provider =>
                            {
                                var options = new SmtpServerOptionsBuilder()
                                    .ServerName("SMTP Server")
                                    .Port(1025)
                                    .Build();

                                return new SmtpServer.SmtpServer(options, provider.GetRequiredService<IServiceProvider>());
                            });

                        services.AddHostedService<Worker>();
                    });

    }
}