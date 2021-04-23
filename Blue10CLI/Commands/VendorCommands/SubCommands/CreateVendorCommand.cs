using Blue10CLI.Services.Interfaces;
using Blue10SDK.Models;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;

namespace Blue10CLI.Commands.VendorCommands
{
    public class CreateVendorCommand : Command
    {
        private readonly IVendorService _vendorService;
        private readonly ILogger<CreateVendorCommand> _logger;

        public CreateVendorCommand(IVendorService vendorService, ILogger<CreateVendorCommand> logger) : base("create", "Creates new vendor in the system")
        {
            _vendorService = vendorService;
            _logger = logger;

            Add(new Option<string>(new[] { "-c", "--company-id" }, "The company/Blue10-administration identifyer under which this vendor will be created") { IsRequired = true });
            Add(new Option<string>(new[] { "-a", "--administration-code" }, "Unique identifyer of Vendor used in ERP") { IsRequired = true });
            Add(new Option<string>("--country", "ISO 3166 two-letter country code of the Vendor's host country") { IsRequired = true });
            Add(new Option<string>("--currency", "ISO 4217 three-letter currency code to set default currency for vendor") { IsRequired = true });
            Add(new Option<string[]>("--iban", "list of IBANs associated with this vendor") { IsRequired = true });

            Add(new Option<string?>(new[] { "-n", "--name" }, "Name of the vendor. Default value will be the administration code"));

            Add(new Option<string>(new[] { "-l", "--ledger" }, () => string.Empty, "Documents from this vendor will be routed to this ledger, leave empty to not associate"));
            Add(new Option<string>(new[] { "-p", "--payment" }, () => string.Empty, "Documents from this vendor will be associated with this payment term, leave empty to not associate"));
            Add(new Option<string>(new[] { "-v", "--vat" }, () => string.Empty, "Documents from this vendor will be associated with this VAT code, leave empty to not associate"));

            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, "Enter path to write output of this command to file. Default output is console only"));

            Handler = CommandHandler.Create<string, string, string, string, string[], string?, string, string, string, EFormatType, FileInfo?>(CreateVendorHandler);
        }

        private async void CreateVendorHandler(
            string companyid,
            string administrationcode,
            string country,
            string currency,
            string[] iban,
            string? name,
            string ledger,
            string payment,
            string vat,
            EFormatType format,
            FileInfo? output)
        {
            var fCreateVendor = new Vendor
            {
                Name = name ?? administrationcode,
                VatNumber = string.Empty,
                CountryCode = country,
                Iban = iban.ToList(),
                CurrencyCode = currency,
                VendorCustomerCode = string.Empty,
                DefaultLedgerCode = ledger,
                DefaultVatCode = vat,
                DefaultVatScenarioCode = string.Empty,
                DefaultPaymentTermCode = payment,
                Blocked = false,
                Id = Guid.Empty,
                AdministrationCode = administrationcode,
                IdCompany = companyid
            };

            var fResult = await _vendorService.CreateOrUpdate(fCreateVendor);

            if (fResult.Object is null)
            {
                _logger.LogWarning($"Creating vendor failed with following error(s): {fResult.ErrorMessage}");
                return;
            }

            format.HandleOutput(fResult.Object, output).Wait();
        }
    }
}