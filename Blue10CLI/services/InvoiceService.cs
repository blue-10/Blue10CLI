using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blue10CLI.models;
using Blue10SDK;
using Blue10SDK.Models;
using Microsoft.Extensions.Logging;

namespace Blue10CLI.services
{
    public class InvoiceService
    {
        
        private readonly ILogger<VendorService> _logger;
        private readonly IBlue10AsyncClient _blue10;


        public InvoiceService(ILogger<VendorService> logger, IBlue10AsyncClient blue10)
        {
            _logger = logger;
            _blue10 = blue10;
        }

        public async Task<IList<DocumentAction>> GetNewPostInvoiceAction()
        {
            var documentActions = await _blue10.GetDocumentActionsAsync();
            var res = documentActions
                .Where(x => x.Action == EDocumentAction.post_purchase_invoice)
                //.Where(x => !x.Status.Equals("Done",StringComparison.OrdinalIgnoreCase))
                //.Where(x=> x.Status.Equals("New",StringComparison.OrdinalIgnoreCase) || x.Status.Equals("WaitingForErp",StringComparison.OrdinalIgnoreCase))
                //.OrderBy(x => x.CreationTime)
                //.GroupBy(x => x.PurchaseInvoice.Id)
                //.Select(x => x.First())
                .ToList();
            return res;
        }
        
        public async Task<IList<PurchaseInvoice>> GetInvoicesToBePosted()
        {
            var invoiceActions = await GetNewPostInvoiceAction();
            return invoiceActions.Select(x=>x.PurchaseInvoice).ToList();
        }
        
        public async Task<IList<ShortInvoice>> PeekInvoices()
        {
            var res = await GetInvoicesToBePosted();
            return res.Select(x => new ShortInvoice(x)).ToList();
        }
        public async Task<(PurchaseInvoice,byte[])> PullInvoice(DocumentAction action)
        {
            var invoice = await _blue10.GetPurchaseInvoiceAsync(action.PurchaseInvoice.Id);
            var data = await _blue10.GetPurchaseInvoiceOriginalAsync(action.PurchaseInvoice.Id);

            //Todo check what should be done 
            action.Result = "Success";
            action.Status = "Done";
            var updateResult = await _blue10.EditDocumentActionAsync(action);
            
            return (invoice,data);
            
        }
    }
}