using Blue10CLI.commands;
using Blue10CLI.commands.credentials;
using Blue10CLI.services;
using Blue10SDK.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Threading.Tasks;

namespace Blue10CLI
{
    class Program
    {
        static async Task Main(params string[] args)
        {
            var key = CredentialsService.EnsureApiKey();
            if (string.IsNullOrWhiteSpace(key)) return;
            var serviceProvider = new ServiceCollection()
                .AddBlue10(key, "https://b10imdev-weu-api.azurewebsites.net")

                //Business Services
                .AddSingleton<InvoiceService>()
                .AddSingleton<InvoiceCommand>()
                    .AddSingleton<PeekInvoiceCommand>()
                    .AddSingleton<PullInvoicesCommand>()
                    .AddSingleton<SignInvoiceCommand>()

                .AddSingleton<VendorService>()
                .AddSingleton<VendorCommand>()
                    .AddSingleton<CreateVendorCommand>()
                    .AddSingleton<ShowVendorCommand>()
                    .AddSingleton<ListVendorsCommand>()
                    .AddSingleton<SyncVendorsCommand>()
                    .AddSingleton<DeleteVendorCommand>()

                .AddSingleton<CompanyService>()
                .AddSingleton<AdministrationCommand>()
                    .AddSingleton<ListCompaniesCommand>()

                .AddSingleton<CredentialsService>()
                .AddSingleton<CredentialsCommand>()
                    .AddSingleton<CheckCredentialsCommand>()
                    .AddSingleton<ShowCredentialsCommand>()
                    .AddSingleton<ClearCredentialsCommand>()
                    .AddSingleton<SetCredentialsCommand>()

                //Commands
                .AddSingleton<Root>()
                .AddLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddFile("blue10_cli.log", append: true);
                    logging.SetMinimumLevel(LogLevel.Warning);
                })
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            //logger.LogDebug("Starting application");
            //do the actual work here
            var root = serviceProvider.GetService<Root>();
            await root.InvokeAsync(args);
        }
    }
}


