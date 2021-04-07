using Blue10CLI.services;
using Blue10SDK.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.commands
{
    public class SyncVendorsCommand : Command
    {
        private readonly VendorService _vendorService;

        public SyncVendorsCommand(VendorService vendorService) : base("sync",
            Descriptions.SyncDescription)
        {
            _vendorService = vendorService;

            Add(new Option<FileInfo?>(
                new[] { "-i", "--input" },
                () => null,
                Descriptions.InputDescription)
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
                    EFormatType.JSON => JsonConvert.DeserializeObject<IList<Vendor>>(fVendorList), //Newtonsoft.Json.JsonSerializationException:
                    EFormatType.CSV => Read.CsvRecords<Vendor>(fVendorList, ","), // CsvHelper.ReaderException:
                    EFormatType.TSV => Read.CsvRecords<Vendor>(fVendorList, "\t"), // CsvHelper.ReaderException
                    EFormatType.SSV => Read.CsvRecords<Vendor>(fVendorList, ";"), // CsvHelper.ReaderException
                    EFormatType.XML => Read.XmlRecords<Vendor>(fVendorList), //System.InvalidOperationException:
                    _ => throw new ArgumentOutOfRangeException(nameof(inputformat), inputformat, null)
                };
            }
            catch (Exception ex) when (
                ex is JsonSerializationException
                || ex is CsvHelper.ReaderException
                || ex is InvalidOperationException)
            {
                Console.WriteLine("Invalid input file. Check if format of the file is correct and if Id values of vendors are valid");
                throw;
            }

            var fSuccessList = new List<Vendor>();
            var fFailedList = new List<Vendor>();

            var fCount = 1;
            var fTotalVendors = fVendors.Count;
            foreach (var fVendor in fVendors)
            {
                var fResult = await _vendorService.CreateOrUpdate(fVendor);
                if (fResult.Vendor == null)
                {
                    fFailedList.Add(fVendor);
                    Console.WriteLine($"{fCount}/{fTotalVendors}: Failed syncing vendor '{fVendor.Name}' - {fResult.ErrorMessage}");
                }
                else
                {
                    fSuccessList.Add(fResult.Vendor);
                    Console.WriteLine($"{fCount}/{fTotalVendors} Successfully synced vendor '{fVendor.Name}'");
                }
                fCount++;
            }

            outputformat.HandleOutput(fSuccessList, output).Wait();
            if (output != null)
            {
                outputformat.HandleOutputToFilePath(fFailedList, $"{output?.Directory?.FullName}/failed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}").Wait();
                outputformat.HandleOutputToFilePath(fSuccessList, $"{output?.Directory?.FullName}/succeed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}").Wait();
            }
            Console.WriteLine($"{fSuccessList.Count}/{fTotalVendors} vendors have been successfully imported");
        }
    }
}