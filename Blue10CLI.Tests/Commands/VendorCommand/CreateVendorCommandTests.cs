using AutoFixture.Xunit2;
using Blue10CLI.commands;
using Blue10CLI.Services.Interfaces;
using Blue10SDK.Models;
using FluentAssertions;
using NSubstitute;
using Objectivity.AutoFixture.XUnit2.AutoNSubstitute.Attributes;
using System;
using System.Collections.Generic;
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
                .Create(Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<IEnumerable<string>>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<string>(),
                Arg.Any<bool>(),
                Arg.Any<string>(),
                Arg.Any<string>())
                .Returns(pVendor);

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
            pVendorService.Received().Create(
                Arg.Is<string>(x => x.Equals(pVendor.AdministrationCode)),
                Arg.Any<string>(),
                Arg.Is<string>(x => x.Equals(pVendor.CountryCode)),
                Arg.Is<IEnumerable<string>>(x => Enumerable.SequenceEqual(x, pVendor.Iban.ToArray())),
                Arg.Is<string>(x => x.Equals(pVendor.CurrencyCode)),
                Arg.Any<string>(),
                Arg.Is<string>(x => x.Equals(pVendor.DefaultLedgerCode)),
                Arg.Is<string>(x => x.Equals(pVendor.DefaultVatCode)),
                Arg.Any<string>(),
                Arg.Is<string>(x => x.Equals(pVendor.DefaultPaymentTermCode)),
                Arg.Is<bool>(x => x.Equals(false)),
                Arg.Is<string>(x => x.Equals(pVendor.AdministrationCode)),
                Arg.Is<string>(x => x.Equals(pVendor.IdCompany)));
        }
    }
}
