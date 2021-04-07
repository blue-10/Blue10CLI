using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using System.Xml.XPath;
using Blue10CLI.services;
using Blue10SDK;
using Microsoft.Extensions.Logging;

namespace Blue10CLI.commands
{
    public class PeekInvoiceCommand : Command
    {
        private readonly InvoiceService _service;
        private readonly ILogger<PeekInvoiceCommand> _logger;

        public PeekInvoiceCommand(InvoiceService service, ILogger<PeekInvoiceCommand> logger) : base("peek", "Peek invoices to be posted")
        {
            _service = service;
            _logger = logger;
            Add(new Option<string?>(new []{"-q","--query"}, () => null,"A query used to filter out results. NOTE: Dependant on output format. If output is 'json', this is a JMESPath query to filter results. https://jmespath.org/. If output is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp"));
            Add(new Option<EFormatType>(new []{"-f","--format"}, () => EFormatType.JSON, "Output format."));
            //Add(new Option<EDocumentAction>(new []{"-a","--last-action"}, () => EDocumentAction.post_purchase_invoice, "Output format."));
            Add(new Option<FileInfo?>(new []{"-o","--output"}, () => null, "Enter path to write output of this command to file. Default output is console only"));
            Handler = CommandHandler.Create<string?,EFormatType,FileInfo?>(PeekInvoiceHandler);
        }


        public async Task PeekInvoiceHandler(string? query,EFormatType format, FileInfo? outputFile)
        {
            var resultObject = await _service.PeekInvoices();
            try
            {
                await format.HandleOutput(resultObject, outputFile, query);
            }
            catch (XPathException xpe)
            {
                _logger.LogError("Filter '{0}' is not a valid XPATH",query,xpe.Message);
            }
        }
    }
}