using AutoFixture.Xunit2;
using Blue10CLI.models;
using Blue10CLI.services;
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
    public class VatCodeServiceTests
    {
        [Theory]
        [AutoMockData]
        public async Task CreateOrUpdate_Update_Success(
            VatCode pVatCodeInput,
            VatCode pVatCodeResult,
            [Frozen] IBlue10AsyncClient pBlue10AsyncCLient,
            VatCodeService pVatCodeService)
        {
            // Setup services
            pBlue10AsyncCLient.EditVatCodeAsync(Arg.Any<VatCode>()).Returns(pVatCodeResult);

            // Test
            var fResult = await pVatCodeService.CreateOrUpdate(pVatCodeInput);

            // Validate
            pBlue10AsyncCLient.Received(1);
            await pBlue10AsyncCLient.Received().EditVatCodeAsync(Arg.Is<VatCode>(x => x.Equals(pVatCodeInput)));
            fResult.Should().BeOfType<BaseResultModel<VatCode>>();
            fResult.ErrorMessage.Should().BeNull();
            fResult.Object.Should().Be(pVatCodeResult);
        }

        [Theory]
        [AutoMockData]
        public async Task CreateOrUpdate_Create_Success(
            VatCode pVatCodeInput,
            VatCode pVatCodeResult,
            [Frozen] IBlue10AsyncClient pBlue10AsyncCLient,
            VatCodeService pVatCodeService)
        {
            // Setup data
            pVatCodeInput.Id = Guid.Empty;

            // Setup services
            pBlue10AsyncCLient.AddVatCodeAsync(Arg.Any<VatCode>()).Returns(pVatCodeResult);

            // Test
            var fResult = await pVatCodeService.CreateOrUpdate(pVatCodeInput);

            // Validate
            pBlue10AsyncCLient.Received(1);
            await pBlue10AsyncCLient.Received().AddVatCodeAsync(Arg.Is<VatCode>(x => x.Equals(pVatCodeInput)));
            fResult.Should().BeOfType<BaseResultModel<VatCode>>();
            fResult.ErrorMessage.Should().BeNull();
            fResult.Object.Should().Be(pVatCodeResult);
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
            VatCode pVatCodeInput,
            [Frozen] IBlue10AsyncClient pBlue10AsyncCLient,
            VatCodeService pVatCodeService)
        {
            // Setup data
            if (pAdministrationCode) pVatCodeInput.AdministrationCode = string.Empty;
            if (pIdCompany) pVatCodeInput.IdCompany = string.Empty;
            if (pName) pVatCodeInput.Name = string.Empty;

            // Setup validatione
            var fErrors = new List<string>();
            if (pAdministrationCode) fErrors.Add("AdministrationCode is empty");
            if (pIdCompany) fErrors.Add("IdCompany is empty");
            if (pName) fErrors.Add("Name is empty");
            var fExpected = string.Join(", ", fErrors);

            // Test
            var fResult = await pVatCodeService.CreateOrUpdate(pVatCodeInput);

            // Validate
            pBlue10AsyncCLient.DidNotReceive();
            fResult.Should().BeOfType<BaseResultModel<VatCode>>();
            fResult.Object.Should().BeNull();
            fResult.ErrorMessage.Should().Be(fExpected);
        }
    }
}
