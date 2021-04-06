using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Blue10CLI.services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Blue10CLI.commands
{
    public class SignInvoice : Command
    {
        private InvoiceService _service;
        private ILogger<PullInvoices> _logger;

        
        
        public SignInvoice(InvoiceService service, ILogger<PullInvoices> logger) : base("sign", "Sign-off invoices with a ledger entry number")
        {
            _service = service;
            _logger = logger;
            Add(new Option<Guid>(new []{"-i","--invoice-id", "--invoice"},"The Id of the invoice to be signed off"){IsRequired = true});
            Add(new Option<string>(new []{"-c","--ledger-entry-code","--entry"},"The ledger entry code assigned by the invoice by the ERP system"){IsRequired = true});
            
            Add(new Option<EFormatType>(new []{"-f","--format"}, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new []{"-o","--output"}, () => null, "Enter path to write output of this command to file. Default output is console only"));
            Handler = CommandHandler.Create<Guid,string,EFormatType,FileInfo?>(SignInvoiceHandler);
        }
        
        public async Task SignInvoiceHandler(Guid invoice, string entry, EFormatType format, FileInfo? output)
        {

            var invoiceActions = await _service.GetNewPostInvoiceAction();
            var targetInvoiceAction = invoiceActions.FirstOrDefault(x => x.Id == invoice);

            if (targetInvoiceAction == null)
            {
                _logger.LogError($"Invoice with id {invoice} does not exist, is not ready to be posted or has already been signed off");
                return;
            }

            var res = await _service.SignInvoice(targetInvoiceAction, entry);
            if (res != null)
            {
                await format.HandleOutput(res, output);
            }
            else
            {
                _logger.LogError($"Failed to sign-off invoice with id {invoice.ToString()}");
            }
        }
    }
}