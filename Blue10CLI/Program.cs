using Blue10CLI.commands;
using Blue10CLI.commands.credentials;
using Blue10CLI.Helpers;
using Blue10CLI.services;
using Blue10CLI.Services.Interfaces;
using Blue10SDK.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace Blue10CLI
{
    class Program
    {
        static async Task Main(params string[] args)
        {
            // StartUp
            AppConfiguration.GetSettings();
            var fApiKey = CredentialsService.EnsureApiKey();
            if (string.IsNullOrWhiteSpace(fApiKey))
            {
                Console.WriteLine("No Blue10 API Key found. Execution ended");
                return;
            }

            // Setup serrvices
            var serviceProvider = new ServiceCollection()
                .AddBlue10(fApiKey, AppConfiguration.Values.BaseUrl)

                //Business Services
                .AddSingleton<InvoiceService>()
                .AddSingleton<InvoiceCommand>()
                    .AddSingleton<PeekInvoiceCommand>()
                    .AddSingleton<PullInvoicesCommand>()
                    .AddSingleton<SignInvoiceCommand>()

                .AddSingleton<IVendorService, VendorService>()
                .AddSingleton<VendorCommand>()
                    .AddSingleton<CreateVendorCommand>()
                    .AddSingleton<ShowVendorCommand>()
                    .AddSingleton<ListVendorsCommand>()
                    .AddSingleton<SyncVendorsCommand>()
                    .AddSingleton<DeleteVendorCommand>()

                .AddSingleton<IGLAccountService, GLAccountService>()
                .AddSingleton<GLAccountCommand>()
                    .AddSingleton<ListGLAccountsCommand>()
                    .AddSingleton<SyncGLAccountsCommand>()

                .AddSingleton<IVatCodeService, VatCodeService>()
                .AddSingleton<VatCodeCommand>()
                    .AddSingleton<ListVatCodesCommand>()
                    .AddSingleton<SyncVatCodesCommand>()

                .AddSingleton<CompanyService>()
                .AddSingleton<AdministrationCommand>()
                    .AddSingleton<ListCompaniesCommand>()

                .AddSingleton<CredentialsService>()
                .AddSingleton<CredentialsCommand>()
                    .AddSingleton<CheckCredentialsCommand>()
                    .AddSingleton<ShowCredentialsCommand>()
                    .AddSingleton<ClearCredentialsCommand>()
                    .AddSingleton<SetCredentialsCommand>()

                // Commands
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

            // Start root command
            var root = serviceProvider.GetService<Root>();
            if (root != null)
                await root.InvokeAsync(args);
        }
    }
}


