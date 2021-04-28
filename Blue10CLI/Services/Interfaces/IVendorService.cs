using Blue10CLI.Models;
using Blue10SDK.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blue10CLI.Services.Interfaces
{
    public interface IVendorService
    {
        Task<IList<Vendor>> List(string companyId);

        Task<BaseResultModel<Vendor>> CreateOrUpdate(Vendor pVendor);
    }
}