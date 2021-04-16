using Blue10CLI.Helpers;
using Blue10CLI.Services.Interfaces;
using Blue10SDK.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.Commands.VatCodeCommands
{
    public class SyncVatCodesCommand : Command
    {
        private readonly IVatCodeService _vatCodeService;

        public SyncVatCodesCommand(IVatCodeService vatCodeService) : base("sync",
            Descriptions.SyncVatCodeDescription)
        {
            _vatCodeService = vatCodeService;

            Add(new Option<FileInfo?>(
                new[] { "-i", "--input" },
                () => null,
                Descriptions.InputVatCodeDescription)
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

            Handler = CommandHandler.Create<FileInfo, EFormatType, FileInfo?, EFormatType>(ImportVatCodesHandler);
        }

        private async Task ImportVatCodesHandler(
            FileInfo input,
            EFormatType inputformat,
            FileInfo? output,
            EFormatType outputformat)
        {
            var fSyncFilePath = input.FullName;
            var fVatCodeList = File.ReadAllText(fSyncFilePath);

            IList<VatCode> fVatCodes;

            try
            {
                fVatCodes = inputformat switch
                {
                    EFormatType.JSON => JsonConvert.DeserializeObject<IList<VatCode>>(fVatCodeList),
                    EFormatType.CSV => Read.CsvRecords<VatCode>(fVatCodeList, ","),
                    EFormatType.TSV => Read.CsvRecords<VatCode>(fVatCodeList, "\t"),
                    EFormatType.SSV => Read.CsvRecords<VatCode>(fVatCodeList, ";"),
                    EFormatType.XML => Read.XmlRecords<VatCode>(fVatCodeList),
                    _ => throw new ArgumentOutOfRangeException(nameof(inputformat), inputformat, null)
                };
            }
            catch (Exception ex) when (
                ex is JsonSerializationException
                || ex is CsvHelper.ReaderException
                || ex is InvalidOperationException)
            {
                Console.WriteLine("Invalid input file. Check if format of the file is correct and if Id values of VATCodes are valid");
                throw;
            }

            var fSuccessList = new List<VatCode>();
            var fFailedList = new List<VatCode>();

            var fCount = 1;
            var fTotalVATCodes = fVatCodes.Count;
            foreach (var fVatCode in fVatCodes)
            {
                var fResult = await _vatCodeService.CreateOrUpdate(fVatCode);
                if (fResult.Object == null)
                {
                    fFailedList.Add(fVatCode);
                    Console.WriteLine($"{fCount}/{fTotalVATCodes}: Failed syncing VATCode '{fVatCode.Name}' - {fResult.ErrorMessage}");
                }
                else
                {
                    fSuccessList.Add(fResult.Object);
                    Console.WriteLine($"{fCount}/{fTotalVATCodes} Successfully synced VATCode '{fVatCode.Name}'");
                }
                fCount++;
            }

            outputformat.HandleOutput(fSuccessList, output).Wait();
            if (output != null)
            {
                outputformat.HandleOutputToFilePath(fFailedList, $"{output?.Directory?.FullName}/failed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}").Wait();
                outputformat.HandleOutputToFilePath(fSuccessList, $"{output?.Directory?.FullName}/succeed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}").Wait();
            }
            Console.WriteLine($"{fSuccessList.Count}/{fTotalVATCodes} VATCodes have been successfully imported");
        }
    }
}