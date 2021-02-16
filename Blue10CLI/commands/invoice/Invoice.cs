using System.CommandLine;

namespace Blue10CLI.commands
{
    public class Invoice : Command
    {
        public Invoice(
            PeekInvoice peekinvoice,
            PullInvoices pullinvoices
        ) : base("invoice", "creates lists and manages invoices")
        {
            //Basic
            Add(peekinvoice);
            Add(pullinvoices);
        }
    }

   
}