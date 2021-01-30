using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class PullInvoices : Command
    {
        public PullInvoices(InvoiceService service) : base("sync", "Synchronises all Invoices with a provided file")
        {
        }
    }
}