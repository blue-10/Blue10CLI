using Blue10CLI.Enums;
using Blue10CLI.Helpers;
using Blue10CLI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.Commands.VatCodeCommands
{
    public class ListVatCodesCommand : Command
    {
        private readonly IVatCodeService _vatCodeService;
        private readonly IInOutService _utilities;
        private readonly ILogger<ListVatCodesCommand> _logger;

        public ListVatCodesCommand(
            IVatCodeService vatCodeService,
            IInOutService utilities,
            ILogger<ListVatCodesCommand> logger) :
            base("list", "Lists all known VatCodes in a company")
        {
            _vatCodeService = vatCodeService;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<string?>(new[] { "-c", "--company-id" }, () => null, "The company identifier under which the VatCodes exists") { IsRequired = true });
            Add(new Option<string?>(new[] { "-q", "--query" }, () => null, Descriptions.QueryDescription));
            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, Descriptions.FormatDescription));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, Descriptions.OutputDescription));

            Handler = CommandHandler.Create<string, string, EFormatType, FileInfo?>(ListVatCodesHandler);
        }

        private async Task ListVatCodesHandler(string companyid, string? query, EFormatType format, FileInfo? output)
        {
            var resultObject = await _vatCodeService.List(companyid);
            await _utilities.HandleOutput(format, resultObject, output, query);
        }
    }
}