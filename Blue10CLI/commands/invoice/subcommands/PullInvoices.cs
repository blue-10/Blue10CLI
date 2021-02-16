using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Blue10CLI.services;
using Microsoft.Extensions.Logging;

namespace Blue10CLI.commands
{
    public class PullInvoices : Command
    {
        private InvoiceService _service;
        private ILogger<PullInvoices> _logger;

        private const string DEFAULT_DIRECTORY = "./invoices";
        
        public PullInvoices(InvoiceService service, ILogger<PullInvoices> logger) : base("pull", "Peek invoices to be posted")
        {
            _service = service;
            _logger = logger;
            Add(new Option<string?>(new []{"-q","--query"}, () => null,"A query used to filter out results. NOTE: Dependant on output format. If output is 'json', this is a JMESPath query to filter results. https://jmespath.org/. If output is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp"));
            Add(new Option<EFormatType>(new []{"-f","--format"}, () => EFormatType.JSON, "Output format."));
            //Add(new Option<EDocumentAction>(new []{"-a","--last-action"}, () => EDocumentAction.post_purchase_invoice, "Output format."));
            Add(new Option<DirectoryInfo>(new []{"-o","--output"}, () => new DirectoryInfo(DEFAULT_DIRECTORY), "Enter path to write output of this command to the filesystem. Default output will create an 'invoices' directory in the root of the console"));
            Handler = CommandHandler.Create<string?,EFormatType,DirectoryInfo?>(PullInvoiceHandler);
        }
        
        public async Task PullInvoiceHandler(string? query,EFormatType format, DirectoryInfo outputDirectory)
        {

            var invoiceActions = await _service.GetNewPostInvoiceAction();

            foreach (var invoiceAction in invoiceActions)
            {
                var ( purchaseInvoice, data) = await _service.PullInvoice(invoiceAction);

                var outPutFile = outputDirectory?.FullName + purchaseInvoice.Id;

                var extension = format switch
                {
                    EFormatType.JSON => ".json",
                    EFormatType.CSV => ".csv",
                    EFormatType.TSV => ".tsv",
                    EFormatType.SSV => ".ssv",
                    EFormatType.XML => ".xml",
                    _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
                };
                await format.HandleOutput(purchaseInvoice, new FileInfo(outPutFile+extension), query);
                File.WriteAllBytes(outPutFile+".pdf",data);
            }
        }
        
    }
}