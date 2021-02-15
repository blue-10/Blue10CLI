using System.Collections.Generic;
using System.Threading.Tasks;
using Blue10CLI.commands.credentials;
using Blue10SDK;
using Microsoft.Extensions.Logging;

namespace Blue10CLI.services
{
    public class AdministrationService
    {
        
        private readonly ILogger<VendorService> _logger;
        private readonly IBlue10AsyncClient _blue10;
        
        
        public AdministrationService(ILogger<VendorService> logger, IBlue10AsyncClient blue10)
        {
            _logger = logger;
            _blue10 = blue10;
        }

        public async Task<IList<Blue10SDK.Models.Company>> ListAdministrations()
        {
            var res = await _blue10.GetCompaniesAsync();
            return res;
        }
    }
}