using Blue10CLI.Helpers;
using Blue10CLI.Services.Interfaces;
using Blue10SDK.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
        private readonly ILogger<SyncVendorsCommand> _logger;

        public SyncVendorsCommand(
            IVendorService vendorService,
            ILogger<SyncVendorsCommand> logger) :
            base("sync",
                Descriptions.SyncVendorDescription)
        {
            _vendorService = vendorService;
            _logger = logger;

            Add(new Option<FileInfo?>(
                new[] { "-i", "--input" },
                () => null,
                Descriptions.InputVendorDescription)
            { IsRequired = true });
            Add(new Option<EFormatType>(
                new[] { "--input-format" },
                () => EFormatType.JSON,
                Descriptions.InputFormatDescription)
            { IsRequired = true });
            Add(new Option<FileInfo?>(
                new[] { "-o", "--output" },
                () => null,
                Descriptions.OutputDescription));
            Add(new Option<EFormatType>(
                new[] { "-f", "--format", "--output-format" },
                () => EFormatType.JSON,
                Descriptions.FormatDescription));

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

            IList<Vendor> fVendors;

            try
            {
                fVendors = inputformat switch
                {
                    EFormatType.JSON => JsonConvert.DeserializeObject<IList<Vendor>>(fVendorList),
                    EFormatType.CSV => Read.CsvRecords<Vendor>(fVendorList, ","),
                    EFormatType.TSV => Read.CsvRecords<Vendor>(fVendorList, "\t"),
                    EFormatType.SSV => Read.CsvRecords<Vendor>(fVendorList, ";"),
                    EFormatType.XML => Read.XmlRecords<Vendor>(fVendorList),
                    _ => throw new ArgumentOutOfRangeException(nameof(inputformat), inputformat, null)
                };
            }
            catch (Exception ex) when (
                ex is JsonSerializationException
                || ex is CsvHelper.ReaderException
                || ex is InvalidOperationException)
            {
                _logger.LogError("Invalid input file. Check if format of the file is correct and if Id values of vendors are valid");
                throw;
            }

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

            try
            {
                await outputformat.HandleOutput(fSuccessList, output);
                if (output != null)
                {
                    await outputformat.HandleOutputToFilePath(fFailedList, $"{output?.Directory?.FullName}/failed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}");
                    await outputformat.HandleOutputToFilePath(fSuccessList, $"{output?.Directory?.FullName}/succeed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}");
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                _logger.LogError($"{outputformat} is not supported for this action: {e.Message}");
            }
        }
    }
}