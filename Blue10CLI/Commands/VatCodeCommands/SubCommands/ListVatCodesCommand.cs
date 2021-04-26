using Blue10CLI.Enums;
using Blue10CLI.Helpers;
using Blue10CLI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
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
            base("list", "Lists all known VatCodes in administration")
        {
            _vatCodeService = vatCodeService;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<string?>(new[] { "-c", "-a", "--company", "--administration" }, () => null, "The company/Blue10-administration under which this VatCodes exists") { IsRequired = true });
            Add(new Option<string?>(new[] { "-q", "--query" }, () => null, Descriptions.QueryDescription));
            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, Descriptions.FormatDescription));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, Descriptions.OutputDescription));

            Handler = CommandHandler.Create<string, string, EFormatType, FileInfo?>(ListVatCodesHandler);
        }

        private async Task ListVatCodesHandler(string administration, string? query, EFormatType format, FileInfo? output)
        {
            var resultObject = await _vatCodeService.List(administration);
            await _utilities.HandleOutput(format, resultObject, output, query);
        }
    }
}