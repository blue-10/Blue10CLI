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
    public class CostCenterService : ICostCenterService
    {
        private readonly IBlue10AsyncClient _blue10;

        public CostCenterService(IBlue10AsyncClient blue10)
        {
            _blue10 = blue10;
        }

        public async Task<IList<CostCenter>> List(string pCompanyId) =>
            await _blue10.GetCostCentersAsync(pCompanyId);

        public async Task<BaseResultModel<CostCenter>> CreateOrUpdate(CostCenter pCostCenter)
        {
            var fErrors = Validate(pCostCenter);
            if (fErrors.Count > 0)
                return new BaseResultModel<CostCenter>(null, string.Join(", ", fErrors));


            if (pCostCenter.Id != Guid.Empty)
                return await TryVendorTask(_blue10.EditCostCenterAsync(pCostCenter));

            pCostCenter.Id = Guid.NewGuid();
            return await TryVendorTask(_blue10.AddCostCenterAsync(pCostCenter));
        }

        private async Task<BaseResultModel<CostCenter>> TryVendorTask(Task<CostCenter> pTask)
        {
            try
            {
                var fGlAccount = await pTask;
                return new BaseResultModel<CostCenter>(fGlAccount, null);
            }
            catch (Blue10ApiException b10apie)
            {
                return new BaseResultModel<CostCenter>(null, b10apie.Message);
            }
        }

        private List<string> Validate(CostCenter pCostCenter)
        {
            var fErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(pCostCenter.AdministrationCode)) fErrors.Add("AdministrationCode is empty");
            if (string.IsNullOrWhiteSpace(pCostCenter.IdCompany)) fErrors.Add("IdCompany is empty");
            if (string.IsNullOrWhiteSpace(pCostCenter.Name)) fErrors.Add("Name is empty");

            return fErrors;
        }
    }
}