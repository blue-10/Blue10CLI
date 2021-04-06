﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
                .ToList();
            return res;
        }
        
        public async Task<IList<PurchaseInvoice>> GetInvoicesToBePosted()
        {
            var invoiceActions = await GetNewPostInvoiceAction();
            var fRes =invoiceActions.Select(x=>x.PurchaseInvoice).ToList();
            return fRes;
        }
        
        public async Task<IList<InvoiceSummary>> PeekInvoices()
        {
            var res = await GetInvoicesToBePosted();
            return res.Select(x => new InvoiceSummary(x)).ToList();
        }

        public static HttpClient _http = new HttpClient();
        
        public async Task<(PurchaseInvoice,byte[])> PullInvoice(DocumentAction action)
        {
            var invoice = await _blue10.GetPurchaseInvoiceAsync(action.PurchaseInvoice.Id);
            var data = await _blue10.GetPurchaseInvoiceOriginalAsync(action.PurchaseInvoice.Id);
            return (invoice,data);
        }

        public async Task<object> UpdateBookingNr(Guid invoiceId, string bookingNr)
        {
            var post_invoice_actions = await GetNewPostInvoiceAction();
            var target = post_invoice_actions.FirstOrDefault(x => x.PurchaseInvoice.Id == invoiceId);
            if (target is null)
            {
                return "No invoice action for an invoice with this id has been found";
            }
            else
            {
                target.Status = "Done";
                return _blue10.EditDocumentActionAsync(target);
            }
        }

        public async Task<DocumentAction?> SignInvoice(DocumentAction action, string ledgerCode)
        {
            //Todo check what should be done 
            action.Result = "success";
            action.Status = "done";
            action.PurchaseInvoice.AdministrationCode = ledgerCode;
            var updateResult = await _blue10.EditDocumentActionAsync(action);
            if (updateResult != null)
            {
                return action;
            }
            else
            {
                return null;
            }
        }
    }
}