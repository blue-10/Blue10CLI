using Blue10CLI.models;
using Blue10CLI.Services.Interfaces;
using Blue10SDK;
using Blue10SDK.Exceptions;
using Blue10SDK.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blue10CLI.services
{
    public class GLAccountService : IGLAccountService
    {
        private readonly ILogger<GLAccountService> _logger;
        private readonly IBlue10AsyncClient _blue10;

        public GLAccountService(ILogger<GLAccountService> logger, IBlue10AsyncClient blue10)
        {
            _logger = logger;
            _blue10 = blue10;
        }

        public async Task<IList<GLAccount>> List(string pCompanyId) =>
            await _blue10.GetGLAccountsAsync(pCompanyId); // try catch?

        public async Task<BaseResultModel<GLAccount>> CreateOrUpdate(GLAccount pGlAccount)
        {
            var fErrors = Validate(pGlAccount);
            if (fErrors.Count > 0)
                return new BaseResultModel<GLAccount>(null, string.Join(", ", fErrors));


            if (pGlAccount.Id != Guid.Empty)
                return await TryVendorTask(_blue10.EditGLAccountAsync(pGlAccount));

            pGlAccount.Id = Guid.NewGuid();
            return await TryVendorTask(_blue10.AddGLAccountAsync(pGlAccount));
        }

        private async Task<BaseResultModel<GLAccount>> TryVendorTask(Task<GLAccount> pTask)
        {
            try
            {
                var fGlAccount = await pTask;
                return new BaseResultModel<GLAccount>(fGlAccount, null);
            }
            catch (Blue10ApiException b10apie)
            {
                return new BaseResultModel<GLAccount>(null, b10apie.Message);
            }
        }

        private List<string> Validate(GLAccount pGlAccount)
        {
            var fErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(pGlAccount.AdministrationCode)) fErrors.Add("AdministrationCode is empty");
            if (string.IsNullOrWhiteSpace(pGlAccount.IdCompany)) fErrors.Add("IdCompany is empty");
            if (string.IsNullOrWhiteSpace(pGlAccount.Name)) fErrors.Add("Name is empty");

            return fErrors;
        }
    }
}