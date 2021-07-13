using Blue10CLI.Enums;
using Blue10CLI.Services.Interfaces;
using Blue10SDK.Models;
using Microsoft.Extensions.Logging;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blue10CLI.Commands.VendorCommands
{
    public class CreateVendorCommand : Command
    {
        private readonly IVendorService _vendorService;
        private readonly IInOutService _utilities;
        private readonly ILogger<CreateVendorCommand> _logger;

        public CreateVendorCommand(
            IVendorService vendorService,
            IInOutService utilities,
            ILogger<CreateVendorCommand> logger) :
            base("create", "Creates new vendor in the system")
        {
            _vendorService = vendorService;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<string>(new[] { "-c", "--company-id" }, "The company identifyer under which this vendor will be created") { IsRequired = true });
            Add(new Option<string>(new[] { "-a", "--administration-code" }, "Unique identifyer of Vendor used in ERP") { IsRequired = true });
            Add(new Option<string>("--country", "ISO 3166 two-letter country code of the Vendor's host country") { IsRequired = true });
            Add(new Option<string>("--currency", "ISO 4217 three-letter currency code to set default currency for vendor") { IsRequired = true });
            Add(new Option<string[]>("--iban", "list of IBANs associated with this vendor") { IsRequired = true });

            Add(new Option<string?>(new[] { "-n", "--name" }, "Name of the vendor. Default value will be the administration-code"));

            Add(new Option<string>(new[] { "-l", "--ledger" }, () => string.Empty, "Documents from this vendor will be routed to this ledger, leave empty to not associate"));
            Add(new Option<string>(new[] { "-p", "--payment" }, () => string.Empty, "Documents from this vendor will be associated with this payment term, leave empty to not associate"));
            Add(new Option<string>(new[] { "-v", "--vat" }, () => string.Empty, "Documents from this vendor will be associated with this VAT code, leave empty to not associate"));

            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, "Output format."));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, "Enter path to write output of this command to file. Default output is console only"));

            Handler = CommandHandler.Create<CreateVendorInput>(CreateVendorHandler);
        }

        private class CreateVendorInput
        {
            public string CompanyId { get; set; } = string.Empty;
            public string AdministrationCode { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
            public string Currency { get; set; } = string.Empty;
            public string[] Iban { get; set; } = new string[0];
            public string? Name { get; set; }
            public string Ledger { get; set; } = string.Empty;
            public string Payment { get; set; } = string.Empty;
            public string Vat { get; set; } = string.Empty;
            public EFormatType Format { get; set; }
            public FileInfo? Output { get; set; }
        }

        private async Task CreateVendorHandler(
            CreateVendorInput pComplexType)
        {
            var fCreateVendor = new Vendor
            {
                Name = pComplexType.Name ?? pComplexType.AdministrationCode,
                VatNumber = string.Empty,
                CountryCode = pComplexType.Country,
                Iban = pComplexType.Iban.ToList(),
                CurrencyCode = pComplexType.Currency,
                VendorCustomerCode = string.Empty,
                DefaultLedgerCode = pComplexType.Ledger,
                DefaultVatCode = pComplexType.Vat,
                DefaultVatScenarioCode = string.Empty,
                DefaultPaymentTermCode = pComplexType.Payment,
                Blocked = false,
                Id = Guid.Empty,
                AdministrationCode = pComplexType.AdministrationCode,
                IdCompany = pComplexType.CompanyId
            };

            var fResult = await _vendorService.CreateOrUpdate(fCreateVendor);

            //var fResult = await _vendorService.CreateOrUpdate(fCreateVendor);

            if (fResult.Object is null)
            {
                _logger.LogWarning($"Creating vendor failed with following error(s): {fResult.ErrorMessage}");
                return;
            }

            await _utilities.HandleOutput(pComplexType.Format, fResult.Object, pComplexType.Output);
        }
    }
}