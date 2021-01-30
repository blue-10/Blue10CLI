using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class ListInvoices : Command
    {
        public ListInvoices(InvoiceService service) : base("list", "Lists all known Invoices in environment")
        {
        }
    }
}