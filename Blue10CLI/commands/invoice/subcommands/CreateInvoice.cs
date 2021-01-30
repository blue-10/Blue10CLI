using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class CreateInvoice : Command
    {
        public CreateInvoice(InvoiceService service) : base("create", "Creates new Invoice in the system")
        {
            
        }
    }
}