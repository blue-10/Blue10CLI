using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Blue10CLI.services;
using Newtonsoft.Json;

namespace Blue10CLI.commands
{
    public class ImportVendors : Command
    {
        private readonly VendorService _vendorService;

        public ImportVendors(VendorService vendorService) : base("import",
            "Imports vendors from file and adds or updates each vendor found to the blue 10 environment")
        {
            _vendorService = vendorService;
            Add(new Option<FileInfo?>(new[] {"-l", "--load-file"}, () => null,
                    "Path to import file. File should have the same structure as the result of the 'vendor list' command  ")
                {IsRequired = true});
            Add(new Option<EFormatType>(new[] {"-i", "--input-format"}, () => EFormatType.JSON,
                "Format of the import file") {IsRequired = true});

            Add(new Option<FileInfo?>(new[] {"-o", "--output"}, () => null,
                "Enter path to write output of this command to file. Default output is console only"));
            Add(new Option<EFormatType>(new[] {"-f", "--format"}, () => EFormatType.JSON, "Output format."));

            Handler = CommandHandler.Create<FileInfo, EFormatType, FileInfo?>(ImportVendorsHandler);
        }

        private async Task ImportVendorsHandler(
            FileInfo importFile,
            EFormatType format,
            FileInfo? output
        )
        {
            var path = importFile.FullName;
            var text = File.ReadAllText(path);

            IList<Blue10SDK.Models.Vendor> incoming_vendors = format switch
            {
                EFormatType.JSON => JsonConvert.DeserializeObject<IList<Blue10SDK.Models.Vendor>>(text),
                EFormatType.CSV => Read.CsvRecords<Blue10SDK.Models.Vendor>(text, ","),
                EFormatType.TSV => Read.CsvRecords<Blue10SDK.Models.Vendor>(text, "\t"),
                EFormatType.SSV => Read.CsvRecords<Blue10SDK.Models.Vendor>(text, ";"),
                EFormatType.XML => Read.XmlRecords<Blue10SDK.Models.Vendor>(text),
                _ => throw new ArgumentOutOfRangeException(nameof(format), format, null)
            };

            var success = new List<Blue10SDK.Models.Vendor>();
            var failed = new List<Blue10SDK.Models.Vendor>();

            var count = 1;
            foreach (var vendor in incoming_vendors)
            {
                var vendor_res = await _vendorService.PushVendor(vendor);
                if (vendor_res == null)
                {
                    failed.Add(vendor);
                    Console.WriteLine($"{count}/{incoming_vendors.Count} Failed pushing vendor '{vendor.Name}'");
                }
                else
                {
                    success.Add(vendor_res);
                    Console.WriteLine($"{count}/{incoming_vendors.Count} Successfully pushed vendor '{vendor.Name}'");
                }
                count++;
            }

            var res = "";
            format.HandleOutput(success, output).Wait();
            if (output != null)
            {
                format.HandleOutputToFilePath(failed, $"{output?.Directory?.FullName}/failed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}").Wait();
            }
            Console.WriteLine($"{success.Count}/{incoming_vendors.Count} vendors have been successfully imported");
        }
    }
}