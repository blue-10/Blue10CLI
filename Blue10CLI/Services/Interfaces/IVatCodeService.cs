using Blue10CLI.Models;
using Blue10SDK.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blue10CLI.Services.Interfaces
{
    public interface IVatCodeService
    {
        Task<IList<VatCode>> List(string pCompanyId);

        Task<BaseResultModel<VatCode>> CreateOrUpdate(VatCode pVatCode);
    }
}