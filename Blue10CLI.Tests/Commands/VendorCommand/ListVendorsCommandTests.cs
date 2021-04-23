using AutoFixture.Xunit2;
using Blue10CLI.Commands.VendorCommands;
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
using Xunit;

namespace Blue10CLI.Tests.Commands.VendorCommand
{
    [Collection("Sequential")]
    public class ListVendorsCommandTests
    {
        [Theory]
        [InlineAutoMockData(EFormatType.JSON)]
        [InlineAutoMockData(EFormatType.CSV)]
        [InlineAutoMockData(EFormatType.SCSV)]
        [InlineAutoMockData(EFormatType.TSV)]
        [InlineAutoMockData(EFormatType.XML)]
        public void Success_ConsolOutput(
            EFormatType pFormat,
            TestConsole pConsoleCommandLine,
            StringWriter pConsole,
            [Frozen] IVendorService pVendorService,
            [Frozen] IList<Vendor> pVendors)
        {
            // Setup data
            pVendorService
                .List(Arg.Any<string>())
                .Returns(pVendors);

            var fCommandLine = $"-a IdCompany -f {pFormat}";

            // Setup services
            var pCommand = new ListVendorsCommand(pVendorService, null);

            // Hook up validation
            Console.SetOut(pConsole);
            var fExpection = pFormat.Format(pVendors);

            // Test
            pCommand.Invoke(fCommandLine, pConsoleCommandLine);

            // Validate
            pConsoleCommandLine.Error.ToString().Should().BeNullOrEmpty();
            pVendorService.Received(1);
            pConsole.ToString().Should().Contain(fExpection);
        }

        [Theory]
        [InlineAutoMockData("-c {0}")]
        [InlineAutoMockData("-a {0}")]
        [InlineAutoMockData("--company {0}")]
        [InlineAutoMockData("--administration {0}")]
        public void Success_ArgumentBinding(
            string pCommandLineTemplate,
            TestConsole pConsoleCommandLine,
            StringWriter pConsole,
            [Frozen] IVendorService pVendorService,
            [Frozen] string pIdCompany)
        {
            // Setup data
            var fCommandLine = string.Format(pCommandLineTemplate,
                pIdCompany);

            // Setup services
            Console.SetOut(pConsole);
            var pCommand = new ListVendorsCommand(pVendorService, null);

            // Test
            pCommand.Invoke(fCommandLine, pConsoleCommandLine);

            // Validate
            pConsoleCommandLine.Error.ToString().Should().BeNullOrEmpty();
            pVendorService.Received(1);
            pVendorService.Received().List(
                Arg.Is<string>(x => x.Equals(pIdCompany)));
        }
    }
}
