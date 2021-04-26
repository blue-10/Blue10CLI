using AutoFixture.Xunit2;
using Blue10CLI.Commands.VendorCommands;
using Blue10CLI.Enums;
using Blue10CLI.Services;
using Blue10CLI.Services.Interfaces;
using Blue10SDK.Models;
using FluentAssertions;
using NSubstitute;
using Objectivity.AutoFixture.XUnit2.AutoNSubstitute.Attributes;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
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
            InOutService pInOutService,
            [Frozen] IVendorService pVendorService,
            [Frozen] IList<Vendor> pVendors)
        {
            // Setup data
            pVendorService
                .List(Arg.Any<string>())
                .Returns(pVendors);

            var fCommandLine = $"-a IdCompany -f {pFormat}";

            // Setup services
            var pCommand = new ListVendorsCommand(pVendorService, pInOutService, null);

            // Test
            pCommand.Invoke(fCommandLine, pConsoleCommandLine);

            // Validate
            pConsoleCommandLine.Error.ToString().Should().BeNullOrEmpty();
            pVendorService.Received(1);
        }

        [Theory]
        [InlineAutoMockData("-c {0}")]
        [InlineAutoMockData("-a {0}")]
        [InlineAutoMockData("--company {0}")]
        [InlineAutoMockData("--administration {0}")]
        public void Success_ArgumentBinding(
            string pCommandLineTemplate,
            TestConsole pConsoleCommandLine,
            InOutService pInOutService,
            [Frozen] IVendorService pVendorService,
            [Frozen] string pIdCompany)
        {
            // Setup data
            var fCommandLine = string.Format(pCommandLineTemplate,
                pIdCompany);

            // Setup services
            var pCommand = new ListVendorsCommand(pVendorService, pInOutService, null);

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
