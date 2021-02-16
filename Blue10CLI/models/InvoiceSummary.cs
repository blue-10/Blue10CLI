using System;
using System.Data.Common;
using Blue10CLI.commands;
using Blue10SDK.Models;

namespace Blue10CLI.models
{
    public class InvoiceSummary
    {
        public Guid Id { get; set; }
        public string InvoiceNr { get; set; }
        public string Description { get; set; }
        public string Vendor { get; set; }
        public DateTime InvoiceDate { get; set; }
        
        
        public InvoiceSummary(){}
        public InvoiceSummary(PurchaseInvoice invoice)
        {
            Id = invoice.Id;
            InvoiceNr = invoice.InvoiceNumber;
            Description = invoice.HeaderDescription;
            Vendor = invoice.VendorCode;
            InvoiceDate = invoice.InvoiceDate;
        }

    }
}