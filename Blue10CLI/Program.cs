using System.CommandLine;
using System.CommandLine.IO;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
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
        /// DragonFruit simple example program
        /// </summary>
        /// <param name="verbose">Show verbose output</param>
        /// <param name="flavor">Which flavor to use</param>
        /// <param name="count">How many smoothies?</param>
        static async Task Main(params string[] args)
        {
            var key = (await File.ReadAllLinesAsync(".credentials")).First();
            using IHost host = CreateHostBuilder(args,key).Build();
            await host.RunAsync();
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
                    //.AddBlue10(key)
                    //Business Services
                    .AddSingleton<InvoiceService>()
                    .AddSingleton<VendorService>()
                    //Commands
                    .AddSingleton<Root>()
                );


    }

    public class Root : RootCommand
    {
        public Root(InvoiceService invoiceService, VendorService vendorService)
        {
            Add(new Vendor(vendorService));
            Add(new Invoice(invoiceService));
        }
    }



}


