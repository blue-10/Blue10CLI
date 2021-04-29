using AutoFixture.Xunit2;
using Blue10CLI.Commands.VendorCommands;
using Blue10CLI.Models;
using Blue10CLI.Services;
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
using Xunit;

namespace Blue10CLI.Tests.Commands.VendorCommand
{
    [Collection("Sequential")]
    public class SyncVendorsCommandTests
    {
        readonly List<Guid> Ids = new List<Guid>{
            Guid.Parse("0611e62c-ea19-4ec4-924b-acd200bfe25d"),
            Guid.Parse("23099de6-19bc-4aed-b6eb-a6d700be8cea"),
            Guid.Parse("42b1db44-90b4-490e-8ba2-ad0200e54f20")};

        [Theory]
        [InlineAutoMockData("-i TestFiles/listVendors.json --input-format JSON")]
        [InlineAutoMockData("-i TestFiles/listVendors.xml --input-format XML")]
        public void Success_ReadAndConvertFiles(
            string pCommandLine,
            TestConsole pConsoleCommandLine,
            InOutService pInOutService,
            [Frozen] IVendorService pVendorService)
        {
            // Setup data
            BaseResultModel<Vendor> fModel = new BaseResultModel<Vendor>(new Vendor(), null);

            // Setup services
            pVendorService
                .CreateOrUpdate(Arg.Any<Vendor>())
                .Returns(fModel);

            var pCommand = new SyncVendorsCommand(pVendorService, pInOutService, null);

            // Test
            pCommand.Invoke(pCommandLine, pConsoleCommandLine);

            // Validate
            pConsoleCommandLine.Error.ToString().Should().BeNullOrEmpty();

            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.Id.Equals(Ids[0])));
            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.Id.Equals(Ids[1])));
            pVendorService.Received().CreateOrUpdate(Arg.Is<Vendor>(x => x.Id.Equals(Ids[2])));
        }
    }
}
