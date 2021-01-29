using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class Invoice : Command
    {
        private readonly InvoiceService _service;

        public Invoice(InvoiceService service) : base("invoice", "creates lists and manages invoices")
        {
            _service = service;
            //Basic
            Add(new CreateInvoice(service));
            Add(new ListInvoices(service));
            Add(new ShowInvoice(service));
            Add(new DeleteInvoice(service));
            Add(new PullInvoices(service));
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