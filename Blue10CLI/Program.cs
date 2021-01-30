using System.CommandLine;
using System.Threading.Tasks;
using Blue10CLI.commands;
using Blue10CLI.services;
using Blue10SDK.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Blue10CLI
{
    class Program
    {
        /// <summary>
        /// Bla
        /// </summary>
        static async Task Main(params string[] args)
        {
            var key = CredentialsService.EnsureApiKey();
            if (string.IsNullOrWhiteSpace(key)) return;
            
            var serviceProvider = new ServiceCollection()
                .AddSingleton<Root>()
                .AddSingleton<Vendor>()
                .AddSingleton<VendorService>()
                .AddSingleton<Invoice>()
                .AddSingleton<InvoiceService>()
                .AddLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddFile("blue10_cli.log", append: true);
                    logging.SetMinimumLevel(LogLevel.Error);
                })
                .BuildServiceProvider();

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();
            logger.LogInformation("Starting application");

            //do the actual work here
            var root = serviceProvider.GetService<Root>();
            await root.InvokeAsync(args);
        }

        static IHostBuilder CreateHostBuilder(string[] args,string key) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddFile("blue10_cli.log", append: true);
                    logging.SetMinimumLevel(LogLevel.Error);
                })
                .ConfigureServices((_, services) => services
                    //Dependencies
                    .AddBlue10(key)
                    //Business Services
                        .AddSingleton<CreateInvoice>()
                        .AddSingleton<ShowInvoice>()
                        .AddSingleton<ListInvoices>()
                        .AddSingleton<PullInvoices>()
                        .AddSingleton<DeleteInvoice>()
                    .AddSingleton<InvoiceService>()
                        
                        .AddSingleton<CreateVendor>()
                        .AddSingleton<ShowVendor>()
                        .AddSingleton<ListVendors>()
                        .AddSingleton<SyncVendors>()
                        .AddSingleton<DeleteVendor>()
                    .AddSingleton<VendorService>()
                    //Commands
                    .AddSingleton<Root>()
                );


    }

    public class Root : RootCommand
    {
        public Root(Vendor vendor, Invoice invoice)
        {
            Add(vendor);
            Add(invoice);
        }
    }



}


