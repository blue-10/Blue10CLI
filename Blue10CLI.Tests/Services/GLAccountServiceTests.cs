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
    public class GLAccountServiceTests
    {
        [Theory]
        [AutoMockData]
        public async Task CreateOrUpdate_Update_Success(
            GLAccount pGLAccountInput,
            GLAccount pGLAccountResult,
            [Frozen] IBlue10AsyncClient pBlue10AsyncCLient,
            GLAccountService pGLAccountService)
        {
            // Setup services
            pBlue10AsyncCLient.EditGLAccountAsync(Arg.Any<GLAccount>()).Returns(pGLAccountResult);

            // Test
            var fResult = await pGLAccountService.CreateOrUpdate(pGLAccountInput);

            // Validate
            pBlue10AsyncCLient.Received(1);
            await pBlue10AsyncCLient.Received().EditGLAccountAsync(Arg.Is<GLAccount>(x => x.Equals(pGLAccountInput)));
            fResult.Should().BeOfType<BaseResultModel<GLAccount>>();
            fResult.ErrorMessage.Should().BeNull();
            fResult.Object.Should().Be(pGLAccountResult);
        }

        [Theory]
        [AutoMockData]
        public async Task CreateOrUpdate_Create_Success(
            GLAccount pGLAccountInput,
            GLAccount pGLAccountResult,
            [Frozen] IBlue10AsyncClient pBlue10AsyncCLient,
            GLAccountService pGLAccountService)
        {
            // Setup data
            pGLAccountInput.Id = Guid.Empty;

            // Setup services
            pBlue10AsyncCLient.AddGLAccountAsync(Arg.Any<GLAccount>()).Returns(pGLAccountResult);

            // Test
            var fResult = await pGLAccountService.CreateOrUpdate(pGLAccountInput);

            // Validate
            pBlue10AsyncCLient.Received(1);
            await pBlue10AsyncCLient.Received().AddGLAccountAsync(Arg.Is<GLAccount>(x => x.Equals(pGLAccountInput)));
            fResult.Should().BeOfType<BaseResultModel<GLAccount>>();
            fResult.ErrorMessage.Should().BeNull();
            fResult.Object.Should().Be(pGLAccountResult);
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
            GLAccount pGLAccountInput,
            [Frozen] IBlue10AsyncClient pBlue10AsyncCLient,
            GLAccountService pGLAccountService)
        {
            // Setup data
            if (pAdministrationCode) pGLAccountInput.AdministrationCode = string.Empty;
            if (pIdCompany) pGLAccountInput.IdCompany = string.Empty;
            if (pName) pGLAccountInput.Name = string.Empty;

            // Setup validatione
            var fErrors = new List<string>();
            if (pAdministrationCode) fErrors.Add("AdministrationCode is empty");
            if (pIdCompany) fErrors.Add("IdCompany is empty");
            if (pName) fErrors.Add("Name is empty");
            var fExpected = string.Join(", ", fErrors);

            // Test
            var fResult = await pGLAccountService.CreateOrUpdate(pGLAccountInput);

            // Validate
            pBlue10AsyncCLient.DidNotReceive();
            fResult.Should().BeOfType<BaseResultModel<GLAccount>>();
            fResult.Object.Should().BeNull();
            fResult.ErrorMessage.Should().Be(fExpected);
        }
    }
}
