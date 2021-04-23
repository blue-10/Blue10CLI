using AutoFixture.Xunit2;
using Blue10CLI.Commands.VendorCommands;
using Blue10CLI.Models;
using Blue10CLI.Services.Interfaces;
using Blue10SDK.Models;
using FluentAssertions;
using NSubstitute;
using Objectivity.AutoFixture.XUnit2.AutoNSubstitute.Attributes;
using System;
using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.IO;
using System.Linq;
using Xunit;

namespace Blue10CLI.Tests.Commands.VendorCommand
{
    [Collection("Sequential")]
    public class CreateVendorCommandTests
    {
        [Theory]
        [InlineAutoMockData(EFormatType.JSON)]
        [InlineAutoMockData(EFormatType.CSV)]
        [InlineAutoMockData(EFormatType.SSV)]
        [InlineAutoMockData(EFormatType.TSV)]
        [InlineAutoMockData(EFormatType.XML)]
        public void Success_ConsolOutput(
            EFormatType pFormat,
            TestConsole pConsoleCommandLine,
            StringWriter pConsole,
            [Frozen] IVendorService pVendorService,
            [Frozen] Vendor pVendor)
        {
            // Setup data
            pVendorService
                .CreateOrUpdate(Arg.Any<Vendor>())
                .Returns(new BaseResultModel<Vendor>(pVendor, null));

            var fCommandLine = $"-c IdCompany -a AdministrationCode --country CountryCode --currency CurrencyCode --iban Iban -f {pFormat}";

            // Setup services
            var pCommand = new CreateVendorCommand(pVendorService, null);

            // Hook up validation
            Console.SetOut(pConsole);

            var fExpection = pFormat.Format(new[] { pVendor });

            // Test
            pCommand.Invoke(fCommandLine, pConsoleCommandLine);

            // Validate
            pConsoleCommandLine.Error.ToString().Should().BeNullOrEmpty();
            pVendorService.Received(1);
            pConsole.ToString().Should().Contain(fExpection);
        }

        [Theory]
        [InlineAutoMockData("-c {0} -a {1} --country {2} --currency {3} --iban {4} -l {5} -p {6} -v {7}")]
        [InlineAutoMockData("--company-id {0} --administration-code {1} --country {2} --currency {3} --iban {4} --ledger {5} --payment {6} --vat {7}")]
        public void Success_ArgumentBinding(
            string pCommandLineTemplate,
            TestConsole pConsoleCommandLine,
            [Frozen] IVendorService pVendorService,
            [Frozen] Vendor pVendor)
        {
            // Setup data
            var fCommandLine = string.Format(pCommandLineTemplate,
                pVendor.IdCompany,
                pVendor.AdministrationCode,
                pVendor.CountryCode,
                pVendor.CurrencyCode,
                String.Join(" ", pVendor.Iban),
                pVendor.DefaultLedgerCode,
                pVendor.DefaultPaymentTermCode,
                pVendor.DefaultVatCode);

            // Setup services
            var pCommand = new CreateVendorCommand(pVendorService, null);

            // Test
            pCommand.Invoke(fCommandLine, pConsoleCommandLine);

            // Validate
            pConsoleCommandLine.Error.ToString().Should().BeNullOrEmpty();
            pVendorService.Received(1);
            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.IdCompany.Equals(pVendor.IdCompany)));
            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.AdministrationCode.Equals(pVendor.AdministrationCode)));
            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.CountryCode.Equals(pVendor.CountryCode)));
            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.CurrencyCode.Equals(pVendor.CurrencyCode)));
            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.Iban.SequenceEqual(pVendor.Iban)));
            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.DefaultLedgerCode.Equals(pVendor.DefaultLedgerCode)));
            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.DefaultPaymentTermCode.Equals(pVendor.DefaultPaymentTermCode)));
            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.DefaultVatCode.Equals(pVendor.DefaultVatCode)));
            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.Name.Equals(pVendor.AdministrationCode)));
        }
    }
}
