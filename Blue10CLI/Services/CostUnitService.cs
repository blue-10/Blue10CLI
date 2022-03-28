using Blue10CLI.Models;
using Blue10CLI.Services.Interfaces;
using Blue10SDK;
using Blue10SDK.Exceptions;
using Blue10SDK.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blue10CLI.Services
{
    public class CostUnitService : ICostUnitService
    {
        private readonly IBlue10AsyncClient _blue10;

        public CostUnitService(IBlue10AsyncClient blue10)
        {
            _blue10 = blue10;
        }

        public async Task<IList<CostUnit>> List(string pCompanyId) =>
            await _blue10.GetCostUnitsAsync(pCompanyId);

        public async Task<BaseResultModel<CostUnit>> CreateOrUpdate(CostUnit pCostUnit)
        {
            var fErrors = Validate(pCostUnit);
            if (fErrors.Count > 0)
                return new BaseResultModel<CostUnit>(null, string.Join(", ", fErrors));


            if (pCostUnit.Id != Guid.Empty)
                return await TryVendorTask(_blue10.EditCostUnitAsync(pCostUnit));

            pCostUnit.Id = Guid.NewGuid();
            return await TryVendorTask(_blue10.AddCostUnitAsync(pCostUnit));
        }

        private async Task<BaseResultModel<CostUnit>> TryVendorTask(Task<CostUnit> pTask)
        {
            try
            {
                var fGlAccount = await pTask;
                return new BaseResultModel<CostUnit>(fGlAccount, null);
            }
            catch (Blue10ApiException b10apie)
            {
                return new BaseResultModel<CostUnit>(null, b10apie.Message);
            }
        }

        private List<string> Validate(CostUnit pCostUnit)
        {
            var fErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(pCostUnit.AdministrationCode)) fErrors.Add("AdministrationCode is empty");
            if (string.IsNullOrWhiteSpace(pCostUnit.IdCompany)) fErrors.Add("IdCompany is empty");
            if (string.IsNullOrWhiteSpace(pCostUnit.Name)) fErrors.Add("Name is empty");

            return fErrors;
        }
    }
}