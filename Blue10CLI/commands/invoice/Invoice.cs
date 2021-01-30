using System.CommandLine;

namespace Blue10CLI.commands
{
    public class Invoice : Command
    {
        public Invoice(
            CreateInvoice createinvoice,
            ListInvoices listinvoices,
            ShowInvoice showinvoice,
            DeleteInvoice deleteinvoice,
            PullInvoices pullinvoices
        ) : base("invoice", "creates lists and manages invoices")
        {
            //Basic
            Add(createinvoice);
            Add(listinvoices);
            Add(showinvoice);
            Add(deleteinvoice);
            Add(pullinvoices);
        }
    }
#region Basic

#endregion
   
}