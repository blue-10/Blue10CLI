using System.CommandLine;

namespace Blue10CLI.Commands.InvoiceCommands
{
    public class InvoiceCommand : Command
    {
        public InvoiceCommand(
            PeekInvoiceCommand peekinvoice,
            PullInvoicesCommand pullinvoices,
            SignInvoiceCommand signinvoice
        ) : base("invoice", "creates lists and manages invoices")
        {
            Add(peekinvoice);
            Add(pullinvoices);
            Add(signinvoice);
        }
    }
}