using System.CommandLine;
using Blue10CLI.services;

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
    public class CreateInvoice : Command
    {
        public CreateInvoice(InvoiceService service) : base("create", "Creates new Invoice in the system")
        {
            
        }
    }

    public class ListInvoices : Command
    {
        public ListInvoices(InvoiceService service) : base("list", "Lists all known Invoices in environment")
        {
        }
    }

    public class ShowInvoice : Command
    {
        public ShowInvoice(InvoiceService service) : base("show", "Shows a single Invoice and it's details")
        {
        }
    }

    public class PullInvoices : Command
    {
        public PullInvoices(InvoiceService service) : base("sync", "Synchronises all Invoices with a provided file")
        {
        }
    }

    public class DeleteInvoice : Command
    {
        public DeleteInvoice(InvoiceService service) : base("delete", "Deletes a Invoice")
        {
        }
    }
    #endregion
   
}