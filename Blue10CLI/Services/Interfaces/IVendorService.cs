using Blue10CLI.models;
using Blue10SDK.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blue10CLI.Services.Interfaces
{
    public interface IVendorService
    {
        Task<IList<Vendor>> List(string companyId);

        Task<Vendor?> Create(
            string pName,
            string pVatNumber,
            string pCountryCode,
            IEnumerable<string> pIban,
            string pCurrencyCode,
            string pVendorCustomerCode,
            string pDefaultLedgerCode,
            string pDefaultVatCode,
            string pDefaultVatScenarioCode,
            string pDefaultPaymentTermCode,
            bool pBlocked,
            string pAdministrationCode,
            string pIdCompany);

        Task<VendorResultModel> CreateOrUpdate(Vendor pVendor);
    }
}