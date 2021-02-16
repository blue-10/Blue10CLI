using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Blue10SDK;
using Blue10SDK.Models;
using DevLab.JmesPath;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace Blue10CLI.services
{
    public class VendorService
    {
        private readonly ILogger<VendorService> _logger;
        private readonly IBlue10AsyncClient _blue10;
        
        
        public VendorService(ILogger<VendorService> logger, IBlue10AsyncClient blue10)
        {
            _logger = logger;
            _blue10 = blue10;
        }

        public async Task<IList<Vendor>> List(string companyId) => 
            await _blue10.GetVendorsAsync(companyId);

        public async Task<Vendor> Create(
            string code,
            string countryCode,
            string currency,
            IEnumerable<string> iban,
            bool blocked,
            string defaultLedger,
            string defaultPaymentTerm,
            string defaultVat,
            string defaultVatScenario
                )
        {
            var newVendor = new Vendor
            {
                AdministrationCode = code,
                Blocked = blocked,
                CountryCode = countryCode,
                DefaultLedgerCode = defaultLedger,
                DefaultPaymentTermCode = defaultPaymentTerm,
                DefaultVatCode = defaultVat,
                DefaultVatScenarioCode = defaultVatScenario,
                CurrencyCode = currency,
                Iban = iban.ToList(),
                Id = Guid.NewGuid()
            };
            return await _blue10.AddVendorAsync(newVendor);;
        }
    }
}