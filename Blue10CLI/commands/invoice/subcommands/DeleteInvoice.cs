using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class DeleteInvoice : Command
    {
        public DeleteInvoice(InvoiceService service) : base("delete", "Deletes a Invoice")
        {
        }
    }
}