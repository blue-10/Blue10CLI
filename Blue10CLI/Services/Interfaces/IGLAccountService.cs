using Blue10CLI.Models;
using Blue10SDK.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blue10CLI.Services.Interfaces
{
    public interface IGLAccountService
    {
        Task<IList<GLAccount>> List(string pCompanyId);

        Task<BaseResultModel<GLAccount>> CreateOrUpdate(GLAccount pGlAccount);
    }
}