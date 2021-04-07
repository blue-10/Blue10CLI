using Blue10CLI.services;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace Blue10CLI.commands
{
    public class CreateVendor : Command
    {
        private readonly VendorService _vendorService;
        private readonly ILogger<CreateVendor> _logger;

        public CreateVendor(VendorService vendorService, ILogger<CreateVendor> logger) : base("create", "Creates new vendor in the system") // Really need to be looked at for the arguments and the assignment
        {
            _vendorService = vendorService;
            _logger = logger;
            Add(new Option<string>(new[] { "-a", "--administration-code" }, "Administration (Company) code under which this vendor will be created") { IsRequired = true }); //Administration code of vendor?
            Add(new Option<string>(new[] { "-c", "--code" }, "Unique Identifyer if Vendor in administration") { IsRequired = true });
            Add(new Option<string>("--country", "ISO 3166 two-letter country code of the Vendor's host country") { IsRequired = true });
            Add(new Option<string>("--currency", "ISO 4217 three-letter currency code to set default currency for vendor") { IsRequired = true });
            Add(new Option<string[]>("--iban", "list of IBANs associated with this vendor") { IsRequired = true });

            Add(new Option<string>(new[] { "-l", "--ledger" }, () => "Documents from this vendor will be routed to this ledger, leave empty to not associate"));
            Add(new Option<string>(new[] { "-p", "--payment" }, () => "Documents from this vendor will be associated with this payment term, leave empty to not associate"));
            Add(new Option<string>(new[] { "-v", "--vat" }, () => "Documents from this vendor will be associated with this VAT code, leave empty to not associate"));
            //Add(new Option<bool>(new []{"-b","--blocked"}, () => false, "Block vendor upon creation, default false"));

            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, "Enter path to write output of this command to file. Default output is console only"));

            Handler = CommandHandler.Create<string, string, string, string, string[], string, string, string, EFormatType, FileInfo?>(CreateVendorHandler);
        }

        private void CreateVendorHandler(
            string administrationcode,
            string code,
            string countryCode,
            string currency,
            string[] iban,
            //bool blocked,
            string ledger,
            string payment,
            string vat,
            EFormatType format,
            FileInfo? output
            )
        {
            var resultObject = _vendorService.Create(code, string.Empty, countryCode, iban, currency, string.Empty, ledger, vat, string.Empty, payment, false, code, administrationcode).Result;
            format.HandleOutput(resultObject, output).Wait();
        }
    }
}