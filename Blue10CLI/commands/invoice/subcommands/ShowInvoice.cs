using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class ShowInvoice : Command
    {
        public ShowInvoice(InvoiceService service) : base("show", "Shows a single Invoice and it's details")
        {
        }
    }
}