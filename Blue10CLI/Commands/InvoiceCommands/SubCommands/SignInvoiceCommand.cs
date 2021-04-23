using Blue10CLI.Services;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blue10CLI.Commands.InvoiceCommands
{
    public class SignInvoiceCommand : Command
    {
        private readonly InvoiceService _service;
        private readonly ILogger<PullInvoicesCommand> _logger;

        public SignInvoiceCommand(InvoiceService service, ILogger<PullInvoicesCommand> logger) : base("sign", "Sign-off invoice with a ledger entry number")
        {
            _service = service;
            _logger = logger;

            Add(new Option<Guid>(new[] { "-i", "--invoice-id" }, "The Id of the invoice to be signed off") { IsRequired = true });
            Add(new Option<string>(new[] { "-c", "--ledger-entry-code" }, "The ledger entry code assigned to the invoice by the ERP system") { IsRequired = true });
            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, "Enter path to write output of this command to file. Default output is console only"));

            Handler = CommandHandler.Create<Guid, string, EFormatType, FileInfo?>(SignInvoiceHandler);
        }

        public async Task SignInvoiceHandler(Guid invoiceId, string ledgerEntryCode, EFormatType format, FileInfo? output)
        {
            var fInvoiceActions = await _service.GetNewPostInvoiceAction();
            var fTargetInvoiceAction = fInvoiceActions.FirstOrDefault(x => x.PurchaseInvoice.Id == invoiceId);

            if (fTargetInvoiceAction == null)
            {
                _logger.LogError($"Invoice with id {invoiceId} does not exist, is not ready to be posted or has already been signed off");
                return;
            }
            var fResult = await _service.SignInvoice(fTargetInvoiceAction, ledgerEntryCode);
            if (fResult != null)
            {
                try
                {
                    await format.HandleOutput(fResult, output);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    _logger.LogError($"{format} is not supported for this action: {e.Message}");
                }
            }
            else
                _logger.LogError($"Failed to sign-off invoice with id {invoiceId}");
        }
    }
}