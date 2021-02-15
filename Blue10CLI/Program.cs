using System.CommandLine;
using System.Threading.Tasks;
using Blue10CLI.commands;
using Blue10CLI.commands.credentials;
using Blue10CLI.services;
using Blue10SDK.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Blue10CLI
{
    class Program
    {
        static async Task Main(params string[] args)
        {
            var key = CredentialsService.EnsureApiKey();
            if (string.IsNullOrWhiteSpace(key)) return;
            var serviceProvider = new ServiceCollection()
                .AddBlue10(key,"https://b10imdev-weu-api.azurewebsites.net")
                
                //Business Services
                .AddSingleton<InvoiceService>()
                .AddSingleton<Invoice>()
                    .AddSingleton<CreateInvoice>()
                    .AddSingleton<ShowInvoice>()
                    .AddSingleton<ListInvoices>()
                    .AddSingleton<PullInvoices>()
                    .AddSingleton<DeleteInvoice>()
                
                .AddSingleton<VendorService>()
                .AddSingleton<Vendor>()        
                    .AddSingleton<CreateVendor>()
                    .AddSingleton<ShowVendor>()
                    .AddSingleton<ListVendors>()
                    .AddSingleton<SyncVendors>()
                    .AddSingleton<DeleteVendor>()
                
                .AddSingleton<AdministrationService>()
                .AddSingleton<Administration>()        
                    .AddSingleton<CreateAdministration>()
                    .AddSingleton<ShowAdministration>()
                    .AddSingleton<ListAdministrations>()
                    .AddSingleton<DeleteAdministration>()
                
                .AddSingleton<CredentialsService>()
                .AddSingleton<Credentials>()
                    .AddSingleton<CheckCredentials>()
                    .AddSingleton<ShowCredentials>()
                    .AddSingleton<ClearCredentials>()
                    .AddSingleton<SetCredentials>()

                //Commands
                .AddSingleton<Root>()
                .AddLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddFile("blue10_cli.log", append: true);
                    logging.SetMinimumLevel(LogLevel.Information);
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


