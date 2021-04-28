using Blue10CLI.Enums;
using Blue10CLI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.Commands.VendorCommands
{
    public class ListVendorsCommand : Command
    {
        private readonly IVendorService _vendorService;
        private readonly IInOutService _utilities;
        private readonly ILogger<ListVendorsCommand> _logger;

        public ListVendorsCommand(
            IVendorService vendorService,
            IInOutService utilities,
            ILogger<ListVendorsCommand> logger) :
            base("list", "Lists all known vendors in environment")
        {
            _vendorService = vendorService;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<string?>(new[] { "-c", "--company-id" }, () => null, "The company identifier under which the vendor exists") { IsRequired = true });
            Add(new Option<string?>(new[] { "-q", "--query" }, () => null, "A query used to filter out results. NOTE: Dependant on output format. If output is 'json', this is a JMESPath query to filter results. https://jmespath.org/. If output is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp"));
            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, "Enter path to write output of this command to file. Default output is console only"));

            Handler = CommandHandler.Create<string, string, EFormatType, FileInfo?>(ListVendorsHandler);
        }

        private async Task ListVendorsHandler(string companyid, string? query, EFormatType format, FileInfo? output)
        {
            var resultObject = await _vendorService.List(companyid);
            await _utilities.HandleOutput(format, resultObject, output, query);
        }
    }
}