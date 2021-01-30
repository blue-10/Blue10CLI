using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Blue10CLI.services;
using Microsoft.VisualBasic.CompilerServices;

namespace Blue10CLI.commands
{
    public class CreateVendor : Command
    {
        private readonly VendorService _vendorService;


        public CreateVendor(VendorService vendorService) : base("create", "Creates new vendor in the system")
        {
            _vendorService = vendorService;
            Add(new Option<string>(new []{"-c","--code"}, "Unique Identifyer if Vendor in administration"){IsRequired = true});
            Add(new Option<string>("--country","ISO 3166 two-letter country code of the Vendor's host country"){IsRequired = true});
            Add(new Option<string>("--currency","ISO 4217 three-letter currency code to set default currency for vendor"){IsRequired = true});
            Add(new Option<string[]>("--iban", "list of IBANs associated with this vendor"){IsRequired = true});

            Add(new Option<string>(new []{"-l","--ledger"}, () => "Documents from this vendor will be routed to this ledger, leave empty to not associate"));
            Add(new Option<string>(new []{"-p","--payment"}, () => "Documents from this vendor will be associated with this payment term, leave empty to not associate"));
            Add(new Option<string>(new []{"-v","--vat"}, () => "Documents from this vendor will be associated with this VAT code, leave empty to not associate"));
            Add(new Option<bool>(new []{"-b","--blocked"}, () => false, "Block vendor upon creation, default false"));
            
            Add(new Option<EFormatType>(new []{"-f","--format"}, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new []{"-o","--output"}, () => null, "Enter path to write output of this command to file. Default output is console only"));

            Handler = CommandHandler.Create<string, string, string, string[], bool, string, string, string, string,EFormatType,FileInfo?>(CreateVendorHandler);
        }

        private async void CreateVendorHandler(
            string code,
            string countryCode,
            string currency,
            string[] iban,
            bool blocked,
            string defaultLedger,
            string defaultPaymentTerm,
            string defaultVat,
            string defaultVatScenario,
            EFormatType format,
            FileInfo? output
            )
        {
            
            var resultObject = await _vendorService.Create(code, countryCode, currency, iban, blocked, defaultLedger, defaultPaymentTerm, defaultVat, defaultVatScenario);
            await format.HandleOutput(resultObject,output);
        }
    }
}