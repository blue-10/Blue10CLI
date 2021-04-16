using Blue10CLI.Services;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.Commands.InvoiceCommands
{
    public class PullInvoicesCommand : Command
    {
        private InvoiceService _service;
        private ILogger<PullInvoicesCommand> _logger;

        private const string DEFAULT_DIRECTORY = "./invoices/";

        public PullInvoicesCommand(InvoiceService service, ILogger<PullInvoicesCommand> logger) : base("pull", "Pull all invoices to be posted")
        {
            _service = service;
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

                var fExtension = format switch
                {
                    EFormatType.JSON => ".json",
                    EFormatType.CSV => ".csv",
                    EFormatType.TSV => ".tsv",
                    EFormatType.SSV => ".ssv",
                    EFormatType.XML => ".xml",
                    _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
                };

                await format.HandleOutput(fPurchaseInvoice, new FileInfo(fFilePath + fExtension), query);
                File.WriteAllBytes(fFilePath + ".pdf", fOriginalFileData);
            }
        }
    }
}