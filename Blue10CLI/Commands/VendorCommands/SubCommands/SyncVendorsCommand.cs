using Blue10CLI.Enums;
using Blue10CLI.Helpers;
using Blue10CLI.Services.Interfaces;
using Blue10SDK.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.Commands.VendorCommands
{
    public class SyncVendorsCommand : Command
    {
        private readonly IVendorService _vendorService;
        private readonly IInOutService _utilities;
        private readonly ILogger<SyncVendorsCommand> _logger;

        public SyncVendorsCommand(
            IVendorService vendorService,
            IInOutService utilities,
            ILogger<SyncVendorsCommand> logger) :
            base("sync",
                Descriptions.SyncVendorDescription)
        {
            _vendorService = vendorService;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<FileInfo?>(new[] { "-i", "--input" }, () => null, Descriptions.InputVendorDescription) { IsRequired = true });
            Add(new Option<EFormatType>(new[] { "--input-format" }, () => EFormatType.JSON, Descriptions.InputFormatDescription) { IsRequired = true });
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, Descriptions.OutputDescription));
            Add(new Option<EFormatType>(new[] { "-f", "--format", "--output-format" }, () => EFormatType.JSON, Descriptions.FormatDescription));

            Handler = CommandHandler.Create<FileInfo, EFormatType, FileInfo?, EFormatType>(ImportVendorsHandler);
        }

        private async Task ImportVendorsHandler(
            FileInfo input,
            EFormatType inputformat,
            FileInfo? output,
            EFormatType outputformat)
        {
            var fSyncFilePath = input.FullName;
            var fVendorList = File.ReadAllText(fSyncFilePath);

            var fVendors = _utilities.ReadAs<Vendor>(inputformat, fVendorList);
            if (fVendors is null)
                return;

            var fSuccessList = new List<Vendor>();
            var fFailedList = new List<Vendor>();

            var fCount = 1;
            var fTotalVendors = fVendors.Count;
            foreach (var fVendor in fVendors)
            {
                var fResult = await _vendorService.CreateOrUpdate(fVendor);
                if (fResult.Object == null)
                {
                    fFailedList.Add(fVendor);
                    _logger.LogWarning($"{fCount}/{fTotalVendors}: Failed syncing vendor '{fVendor.Name}' - {fResult.ErrorMessage}");
                }
                else
                {
                    fSuccessList.Add(fResult.Object);
                    Console.WriteLine($"{fCount}/{fTotalVendors} Successfully synced vendor '{fVendor.Name}'");
                }
                fCount++;
            }

            Console.WriteLine($"{fSuccessList.Count}/{fTotalVendors} vendors have been successfully imported");

            await _utilities.HandleOutput(outputformat, fSuccessList, output);
            if (output != null)
            {
                await _utilities.HandleOutputToFilePath(outputformat, fFailedList, $"{output?.Directory?.FullName}/failed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}");
                await _utilities.HandleOutputToFilePath(outputformat, fSuccessList, $"{output?.Directory?.FullName}/succeed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}");
            }
        }
    }
}