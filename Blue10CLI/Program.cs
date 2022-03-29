using Blue10CLI.Commands;
using Blue10CLI.Commands.CompanyCommands;
using Blue10CLI.Commands.CredentialsCommands;
using Blue10CLI.Commands.GLAccountCommands;
using Blue10CLI.Commands.InvoiceCommands;
using Blue10CLI.Commands.VatCodeCommands;
using Blue10CLI.Commands.VendorCommands;
using Blue10CLI.Services;
using Blue10CLI.Services.Interfaces;
using Blue10SDK.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.Threading.Tasks;
using Blue10CLI.Commands.CostCenterCommands;
using Blue10CLI.Commands.CostUnitCommands;

namespace Blue10CLI
{
    class Program
    {
        static async Task Main(params string[] args)
        {
            // Setup serrvices
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IAppConfigurationService, AppConfigurationService>()
                .AddSingleton<ICredentialsService, CredentialsService>()

                .AddBlue10((serviceProvider, cofiguration) =>
                {
                    var appConfiguration = serviceProvider.GetRequiredService<IAppConfigurationService>();
                    var fApiKey = serviceProvider.GetRequiredService<CredentialsService>().GetApiKey();

                    cofiguration.ApiKey = fApiKey;
                    cofiguration.Url = appConfiguration.GetSettings().BaseUrl;
                })

                .AddSingleton<IInOutService, InOutService>()

                //Business Services
                .AddSingleton<ICostUnitService, CostUnitService>()
                .AddSingleton<CostUnitCommand>()
                    .AddSingleton<ListCostUnitsCommand>()
                    .AddSingleton<SyncCostUnitsCommand>()
                
                
                .AddSingleton<ICostCenterService, CostCenterService>()
                .AddSingleton<CostCenterCommand>()
                    .AddSingleton<ListCostCentersCommand>()
                    .AddSingleton<SyncCostCentersCommand>()
                
                
                .AddSingleton<InvoiceService>()
                .AddSingleton<InvoiceCommand>()
                    .AddSingleton<PeekInvoiceCommand>()
                    .AddSingleton<PullInvoicesCommand>()
                    .AddSingleton<SignInvoiceCommand>()

                .AddSingleton<IVendorService, VendorService>()
                .AddSingleton<VendorCommand>()
                    .AddSingleton<CreateVendorCommand>()
                    .AddSingleton<ListVendorsCommand>()
                    .AddSingleton<SyncVendorsCommand>()

                .AddSingleton<IGLAccountService, GLAccountService>()
                .AddSingleton<GLAccountCommand>()
                    .AddSingleton<ListGLAccountsCommand>()
                    .AddSingleton<SyncGLAccountsCommand>()

                .AddSingleton<IVatCodeService, VatCodeService>()
                .AddSingleton<VatCodeCommand>()
                    .AddSingleton<ListVatCodesCommand>()
                    .AddSingleton<SyncVatCodesCommand>()

                .AddSingleton<CompanyService>()
                .AddSingleton<CompanyCommand>()
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

            // Import Settings
            serviceProvider.GetRequiredService<IAppConfigurationService>()
                .ImportSettings();

            // Check ApiKey credentials
            var fApiKey = serviceProvider.GetRequiredService<CredentialsService>().EnsureApiKey();
            if (string.IsNullOrWhiteSpace(fApiKey))
            {
                logger.LogWarning("No Blue10 API Key found. Execution ended");
                return;
            }

            // Start root command
            var root = serviceProvider.GetService<Root>();
            if (root != null)
                await root.InvokeAsync(args);
            else
                logger.LogWarning("No root found. Execution ended");
        }
    }
}


