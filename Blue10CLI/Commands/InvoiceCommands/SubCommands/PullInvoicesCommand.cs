using Blue10CLI.Enums;
using Blue10CLI.Services;
using Blue10CLI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.Commands.InvoiceCommands
{
    public class PullInvoicesCommand : Command
    {
        private readonly InvoiceService _service;
        private readonly IInOutService _utilities;
        private readonly ILogger<PullInvoicesCommand> _logger;

        private const string DEFAULT_DIRECTORY = "./invoices/";

        public PullInvoicesCommand(
            InvoiceService service,
            IInOutService utilities,
            ILogger<PullInvoicesCommand> logger) :
            base("pull", "Pull all invoices to be posted")
        {
            _service = service;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<string?>(new[] { "-q", "--query" }, () => null, "A query used to filter out results. NOTE: Dependant on output format. If output is 'json', this is a JMESPath query to filter results. https://jmespath.org/. If output is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp"));
            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, "Output format."));
            Add(new Option<DirectoryInfo>(new[] { "-o", "--output" }, () => new DirectoryInfo(DEFAULT_DIRECTORY), "Enter path to write output of this command to the filesystem. Default output will create an 'invoices' directory in the root of the console"));

            Handler = CommandHandler.Create<string?, EFormatType, DirectoryInfo?>(PullInvoiceHandler);
        }

        public async Task PullInvoiceHandler(string? query, EFormatType format, DirectoryInfo output)
        {
            var fInvoiceActions = await _service.GetNewPostInvoiceAction();

            if (!Directory.Exists(output.FullName))
                Directory.CreateDirectory(output.FullName);

            foreach (var fInvoiceAction in fInvoiceActions)
            {
                var (fPurchaseInvoice, fOriginalFileData) = await _service.PullInvoice(fInvoiceAction);

                var fFilePath = Path.Combine(output.FullName, fPurchaseInvoice.Id.ToString());

                var fExtension = _utilities.GetExtension(format);

                var fHasResult = await _utilities.HandleOutput(format, fPurchaseInvoice, new FileInfo(fFilePath + fExtension), query);

                if (fHasResult)
                    File.WriteAllBytes(fFilePath + ".pdf", fOriginalFileData);
            }
        }
    }
}