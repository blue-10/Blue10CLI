﻿using Blue10CLI.Models;
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
    public class VendorService : IVendorService
    {
        private readonly ILogger<VendorService> _logger;
        private readonly IBlue10AsyncClient _blue10;

        public VendorService(ILogger<VendorService> logger, IBlue10AsyncClient blue10)
        {
            _logger = logger;
            _blue10 = blue10;
        }

        public async Task<IList<Vendor>> List(string companyId) =>
            await _blue10.GetVendorsAsync(companyId);

        public async Task<BaseResultModel<Vendor>> CreateOrUpdate(
            Vendor pVendor)
        {
            var fErrors = Validate(pVendor);
            if (fErrors.Count > 0)
                return new BaseResultModel<Vendor>(null, string.Join(", ", fErrors));


            if (pVendor.Id != Guid.Empty)
                return await TryVendorTask(_blue10.EditVendorAsync(pVendor));

            pVendor.Id = Guid.NewGuid();
            return await TryVendorTask(_blue10.AddVendorAsync(pVendor));
        }

        private async Task<BaseResultModel<Vendor>> TryVendorTask(Task<Vendor> pTask)
        {
            try
            {
                var fVendor = await pTask;
                return new BaseResultModel<Vendor>(fVendor, null);
            }
            catch (Blue10ApiException b10apie)
            {
                return new BaseResultModel<Vendor>(null, b10apie.Message);
            }
        }

        // Validation in ApiVendor
        private List<string> Validate(Vendor pVendor)
        {
            var fErrors = new List<string>();
            if (string.IsNullOrWhiteSpace(pVendor.AdministrationCode)) fErrors.Add("AdministrationCode is empty");
            if (string.IsNullOrWhiteSpace(pVendor.IdCompany)) fErrors.Add("IdCompany is empty");
            if (string.IsNullOrWhiteSpace(pVendor.Name)) fErrors.Add("Name is empty");

            return fErrors;
        }
    }
}