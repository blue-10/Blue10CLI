using Blue10CLI.Models;
using Blue10CLI.Services.Interfaces;
using Blue10SDK;
using Blue10SDK.Exceptions;
using Blue10SDK.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blue10CLI.Services
{
    public class VatCodeService : IVatCodeService
    {
        private readonly ILogger<VatCodeService> _logger;
        private readonly IBlue10AsyncClient _blue10;

        public VatCodeService(ILogger<VatCodeService> logger, IBlue10AsyncClient blue10)
        {
            _logger = logger;
            _blue10 = blue10;
        }

        public async Task<IList<VatCode>> List(string pCompanyId) =>
            await _blue10.GetVatCodesAsync(pCompanyId); // try catch?

        public async Task<BaseResultModel<VatCode>> CreateOrUpdate(VatCode pVatCode)
        {
            var fErrors = Validate(pVatCode);
            if (fErrors.Count > 0)
                return new BaseResultModel<VatCode>(null, string.Join(", ", fErrors));


            if (pVatCode.Id != Guid.Empty)
                return await TryVendorTask(_blue10.EditVatCodeAsync(pVatCode));

            pVatCode.Id = Guid.NewGuid();
            return await TryVendorTask(_blue10.AddVatCodeAsync(pVatCode));
        }

        private async Task<BaseResultModel<VatCode>> TryVendorTask(Task<VatCode> pTask)
        {
            try
            {
                var fVatCode = await pTask;
                return new BaseResultModel<VatCode>(fVatCode, null);
            }
            catch (Blue10ApiException b10apie)
            {
                return new BaseResultModel<VatCode>(null, b10apie.Message);
            }
        }

        private List<string> Validate(VatCode pVatCode)
        {
            var fErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(pVatCode.AdministrationCode)) fErrors.Add("AdministrationCode is empty");
            if (string.IsNullOrWhiteSpace(pVatCode.IdCompany)) fErrors.Add("IdCompany is empty");
            if (string.IsNullOrWhiteSpace(pVatCode.Name)) fErrors.Add("Name is empty");

            return fErrors;
        }
    }
}