using Blue10SDK;
using Blue10SDK.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blue10CLI.Services
{
    public class CompanyService
    {
        private readonly IBlue10AsyncClient _blue10;

        public CompanyService(IBlue10AsyncClient blue10)
        {
            _blue10 = blue10;
        }

        public async Task<IList<Company>> ListCompanies() =>
            await _blue10.GetCompaniesAsync();

        public async Task<Company> UpdateCompany(Company company) =>
            await _blue10.UpdateCompanyAsync(company);
    }
}