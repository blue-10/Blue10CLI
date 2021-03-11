using System.CommandLine;

namespace Blue10CLI.commands
{
    public class Invoice : Command
    {
        public Invoice(
            PeekInvoice peekinvoice,
            PullInvoices pullinvoices,
            SignInvoice signinvoice
        ) : base("invoice", "creates lists and manages invoices")
        {
            Add(peekinvoice);
            Add(pullinvoices);
            Add(signinvoice);
        }
    }

   
}