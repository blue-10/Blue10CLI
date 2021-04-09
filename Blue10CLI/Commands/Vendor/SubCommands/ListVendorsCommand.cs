using Blue10CLI.Services.Interfaces;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.commands
{
    public class ListVendorsCommand : Command
    {
        private IVendorService _vendorService;

        public ListVendorsCommand(IVendorService vendorService) : base("list", "Lists all known vendors in environment")
        {

            _vendorService = vendorService;

            Add(new Option<string?>(
                new[] { "-c", "-a", "--company", "--administration" },
                () => null, "The administration under which this vendor exists")
            { IsRequired = true });
            Add(new Option<string?>(
                new[] { "-q", "--query" },
                () => null,
                "A query used to filter out results. NOTE: Dependant on output format. If output is 'json', this is a JMESPath query to filter results. https://jmespath.org/. If output is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp"));
            Add(new Option<EFormatType>(
                new[] { "-f", "--format" },
                () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>
                (new[] { "-o", "--output" },
                () => null,
                "Enter path to write output of this command to file. Default output is console only"));

            Handler = CommandHandler.Create<string, string, EFormatType, FileInfo?>(ListVendorsHandler);
        }

        private async Task ListVendorsHandler(string administration, string? query, EFormatType format, FileInfo? output)
        {
            var resultObject = await _vendorService.List(administration);
            await format.HandleOutput(resultObject, output, query);
        }
    }
}