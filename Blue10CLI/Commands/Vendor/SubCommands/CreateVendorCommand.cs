using Blue10CLI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace Blue10CLI.commands
{
    public class CreateVendorCommand : Command
    {
        private readonly IVendorService _vendorService;
        private readonly ILogger<CreateVendorCommand> _logger;

        public CreateVendorCommand(IVendorService vendorService, ILogger<CreateVendorCommand> logger) : base("create", "Creates new vendor in the system") // Really need to be looked at for the arguments and the assignment
        {

            _vendorService = vendorService;
            _logger = logger;
            Add(new Option<string>(new[] { "-c", "--company-id" }, "Company Id under which this vendor will be created") { IsRequired = true });
            Add(new Option<string>(new[] { "-a", "--administration-code" }, "Unique Identifyer of Vendor in administration") { IsRequired = true });
            Add(new Option<string>("--country", "ISO 3166 two-letter country code of the Vendor's host country") { IsRequired = true });
            Add(new Option<string>("--currency", "ISO 4217 three-letter currency code to set default currency for vendor") { IsRequired = true });
            Add(new Option<string[]>("--iban", "list of IBANs associated with this vendor") { IsRequired = true });

            Add(new Option<string>(new[] { "-l", "--ledger" }, () => string.Empty, "Documents from this vendor will be routed to this ledger, leave empty to not associate"));
            Add(new Option<string>(new[] { "-p", "--payment" }, () => string.Empty, "Documents from this vendor will be associated with this payment term, leave empty to not associate"));
            Add(new Option<string>(new[] { "-v", "--vat" }, () => string.Empty, "Documents from this vendor will be associated with this VAT code, leave empty to not associate"));
            //Add(new Option<bool>(new []{"-b","--blocked"}, () => false, "Block vendor upon creation, default false"));

            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, "Enter path to write output of this command to file. Default output is console only"));

            Handler = CommandHandler.Create<string, string, string, string, string[], string, string, string, EFormatType, FileInfo?>(CreateVendorHandler);
        }

        public void CreateVendorHandler(
            string companyid,
            string administrationcode,
            string country,
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
            var resultObject = _vendorService.Create(administrationcode, string.Empty, country, iban, currency, string.Empty, ledger, vat, string.Empty, payment, false, administrationcode, companyid).Result;
            format.HandleOutput(resultObject, output).Wait();
        }
    }
}