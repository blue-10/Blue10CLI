using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Blue10CLI.services;
using Microsoft.Extensions.Logging;

namespace Blue10CLI.commands
{
    public class UpdateBookingNr : Command
    {
        private InvoiceService _service;
        private ILogger<PullInvoices> _logger;

        private const string DEFAULT_DIRECTORY = "./invoices";
        
        public UpdateBookingNr(InvoiceService service, ILogger<PullInvoices> logger) : base("update", "Add a booking nr to an existing invoice")
        {
            _service = service;
            _logger = logger;
            Add(new Option<string>(new []{"-i","--invoice-id"}, "The Id of the target invoice"){IsRequired = true});
            Add(new Option<string>(new []{"-b","--booking-nr"}, "The booking number that will be associated with the invoice after it has been booked"){IsRequired = true});
            Add(new Option<EFormatType>(new []{"-f","--format"}, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new []{"-o","--output"}, () => null, "Enter path to write output of this command to file. Default output is console only"));
            Handler = CommandHandler.Create<string,string,EFormatType,FileInfo?>(UpdateBookingNrHandler);
        }
        
        public async Task UpdateBookingNrHandler(string invoiceId, string bookingNr,EFormatType format, FileInfo? outputFile)
        {
            //var res = _service.UpdateBookingNr(invoiceId, bookingNr);
            //await format.HandleOutput(res, outputFile, null);
        }
        
    }
}