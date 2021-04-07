using Blue10CLI.services;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;

namespace Blue10CLI.commands
{
    public class CreateVendorCommand : Command
    {
        private readonly VendorService _vendorService;
        private readonly ILogger<CreateVendorCommand> _logger;

        public CreateVendorCommand(VendorService vendorService, ILogger<CreateVendorCommand> logger) : base("create", "Creates new vendor in the system") // Really need to be looked at for the arguments and the assignment
        {
            /* POSTMAN DESCRIPTION 
            <table>
                <tr><th>Fieldname</th><th>Type</th><th>r/w</th><th>Description</th></tr>
                <tr><td>id</td><td>Guid</td><td>r</td><td>Id of the vendor as provided by Blue10 (after adding via the Blue10 API)</td></tr>         
                <tr><td>id_company</td><td>varchar(64)</td><td>r</td><td>Id of the company as set in Blue10. Company to which the vendor is synchronized.</td></tr>
                <tr><td>administration_code</td><td>varchar(64)</td><td>r</td><td>Code/number/id of the vendor in the financial system</td></tr>
                <tr><td>name</td><td>varchar(64)</td><td>r/w</td><td>Name of the vendor in the financial system</td></tr>
                <tr><td>country_code</td><td>varchar(2)</td><td>r/w</td><td>Countrycode, given in ISO 3166 (2 characters)</td></tr>
                <tr><td>vat_number</td><td>varchar(32)</td><td>r/w</td><td>VAT registration number of the vendor</td></tr>
                <tr><td>iban</td><td>string[]</td><td>r/w</td><td>List of IBAN number(s) of the vendor</td></tr>
                <tr><td>currency_code</td><td>varchar(3)</td><td>r/w</td><td>(Default) currency code the vendor invoices in, this can be adjusted per invoice. Gicen in ISO 4217 (3 characters)</td></tr>
                <tr><td>vendor_customer_code</td><td>varchar(32)</td><td>r/w</td><td>Your customer code at the vendor</td></tr>
                <tr><td>default_ledger_code</td><td>varchar(32)</td><td>r/w</td><td>Default GL account for vendor to code on (administration_code of GL account)</td></tr>
                <tr><td>default_vat_code</td><td>varchar(32)</td><td>r/w</td><td>Default VAT code, used when coding lines for VAT code on line (administration_code of VAT code)</td></tr>
                <tr><td>default_vat_scenario_code</td><td>varchar(32)</td><td>r/w</td><td>Default Vat scenario code (administration_code of VAT scenario)</td></tr>
                <tr><td>default_payment_term_code</td><td>varchar(32)</td><td>r/w</td><td>Default payment term code (administration_code of Payment term)</td></tr>
                <tr><td>blocked</td><td>bool</td><td>r</td><td>When the vendor is blocked, no new invoices can be registered to this vendor. Current invoices will remain visible on this vendor</td></tr>
                <tr><td>last_update_date</td><td>datetime</td><td>r</td><td>Date the vendor is last changed in Blue10. This could be used to optimize the synchronization by only changing the vendors that have not been changed in Blue10 after they have been changed in the financial system.</td></tr>
            </table>
            */

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