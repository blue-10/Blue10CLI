using Blue10CLI.Enums;
using Blue10CLI.Helpers;
using Blue10CLI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.Commands.GLAccountCommands
{
    public class ListGLAccountsCommand : Command
    {
        private readonly IGLAccountService _glaccountService;
        private readonly IInOutService _utilities;
        private readonly ILogger<ListGLAccountsCommand> _logger;

        public ListGLAccountsCommand(
            IGLAccountService glaccountService,
            IInOutService utilities,
            ILogger<ListGLAccountsCommand> logger) :
            base("list", "Lists all known GLAccounts in a company")
        {
            _glaccountService = glaccountService;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<string?>(new[] { "-c", "--company-id" }, () => null, "The company identifier under which the GLAccounts exists") { IsRequired = true });
            Add(new Option<string?>(new[] { "-q", "--query" }, () => null, Descriptions.QueryDescription));
            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, Descriptions.FormatDescription));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, Descriptions.OutputDescription));

            Handler = CommandHandler.Create<string, string, EFormatType, FileInfo?>(ListGLAccountsHandler);
        }

        private async Task ListGLAccountsHandler(string companyid, string? query, EFormatType format, FileInfo? output)
        {
            var resultObject = await _glaccountService.List(companyid);
            await _utilities.HandleOutput(format, resultObject, output, query);
        }
    }
}