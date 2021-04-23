using AutoFixture.Xunit2;
using Blue10CLI.Models;
using Blue10CLI.Services;
using Blue10SDK;
using Blue10SDK.Models;
using FluentAssertions;
using NSubstitute;
using Objectivity.AutoFixture.XUnit2.AutoNSubstitute.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Blue10CLI.Tests.Services
{
    public class VendorServiceTests
    {
        [Theory]
        [AutoMockData]
        public async Task CreateOrUpdate_Update_Success(
            Vendor pVendorInput,
            Vendor pVendorResult,
            [Frozen] IBlue10AsyncClient pBlue10AsyncCLient,
            VendorService pVendorService)
        {
            // Setup services
            pBlue10AsyncCLient.EditVendorAsync(Arg.Any<Vendor>()).Returns(pVendorResult);

            // Test
            var fResult = await pVendorService.CreateOrUpdate(pVendorInput);

            // Validate
            pBlue10AsyncCLient.Received(1);
            await pBlue10AsyncCLient.Received().EditVendorAsync(Arg.Is<Vendor>(x => x.Equals(pVendorInput)));
            fResult.Should().BeOfType<BaseResultModel<Vendor>>();
            fResult.ErrorMessage.Should().BeNull();
            fResult.Object.Should().Be(pVendorResult);
        }

        [Theory]
        [AutoMockData]
        public async Task CreateOrUpdate_Create_Success(
            Vendor pVendorInput,
            Vendor pVendorResult,
            [Frozen] IBlue10AsyncClient pBlue10AsyncCLient,
            VendorService pVendorService)
        {
            // Setup data
            pVendorInput.Id = Guid.Empty;

            // Setup services
            pBlue10AsyncCLient.AddVendorAsync(Arg.Any<Vendor>()).Returns(pVendorResult);

            // Test
            var fResult = await pVendorService.CreateOrUpdate(pVendorInput);

            // Validate
            pBlue10AsyncCLient.Received(1);
            await pBlue10AsyncCLient.Received().AddVendorAsync(Arg.Is<Vendor>(x => x.Equals(pVendorInput)));
            fResult.Should().BeOfType<BaseResultModel<Vendor>>();
            fResult.ErrorMessage.Should().BeNull();
            fResult.Object.Should().Be(pVendorResult);
        }

        [Theory]
        [InlineAutoMockData(true, false, false)]
        [InlineAutoMockData(false, true, false)]
        [InlineAutoMockData(false, false, true)]
        [InlineAutoMockData(true, true, false)]
        [InlineAutoMockData(true, false, true)]
        [InlineAutoMockData(false, true, true)]
        [InlineAutoMockData(true, true, true)]
        public async Task CreateOrUpdate_Validate_Failed(
            bool pAdministrationCode,
            bool pIdCompany,
            bool pName,
            Vendor pVendorInput,
            [Frozen] IBlue10AsyncClient pBlue10AsyncCLient,
            VendorService pVendorService)
        {
            // Setup data
            if (pAdministrationCode) pVendorInput.AdministrationCode = string.Empty;
            if (pIdCompany) pVendorInput.IdCompany = string.Empty;
            if (pName) pVendorInput.Name = string.Empty;

            // Setup validatione
            var fErrors = new List<string>();
            if (pAdministrationCode) fErrors.Add("AdministrationCode is empty");
            if (pIdCompany) fErrors.Add("IdCompany is empty");
            if (pName) fErrors.Add("Name is empty");
            var fExpected = string.Join(", ", fErrors);

            // Test
            var fResult = await pVendorService.CreateOrUpdate(pVendorInput);

            // Validate
            pBlue10AsyncCLient.DidNotReceive();
            fResult.Should().BeOfType<BaseResultModel<Vendor>>();
            fResult.Object.Should().BeNull();
            fResult.ErrorMessage.Should().Be(fExpected);
        }
    }
}
