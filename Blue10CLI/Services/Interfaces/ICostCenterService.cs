using Blue10CLI.Models;
using Blue10SDK.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blue10CLI.Services.Interfaces
{
    public interface ICostCenterService
    {
        Task<IList<CostCenter>> List(string pCompanyId);

        Task<BaseResultModel<CostCenter>> CreateOrUpdate(CostCenter pCostCenter);
    }
}