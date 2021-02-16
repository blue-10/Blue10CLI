using System.Collections.Generic;
using System.Threading.Tasks;
using Blue10CLI.commands.credentials;
using Blue10SDK;
using Blue10SDK.Models;
using Microsoft.Extensions.Logging;

namespace Blue10CLI.services
{
    public class CompanyService
    {
        private readonly ILogger<VendorService> _logger;
        private readonly IBlue10AsyncClient _blue10;


        public CompanyService(ILogger<VendorService> logger, IBlue10AsyncClient blue10)
        {
            _logger = logger;
            _blue10 = blue10;
        }

        public async Task<IList<Company>> ListCompanies() =>
            await _blue10.GetCompaniesAsync();

        public async Task<Company> UpdateCompany(Company company) =>
            await _blue10.UpdateCompanyAsync(company);
        
        
        //public async Task<Company> CreateCompany(Company company) => await _blue10.UpdateCompanyAsync(company);
        //public async Task<Company> DeleteCompany(string companyCode) => await _blue10.UpdateCompanyAsync(company);
    }
}