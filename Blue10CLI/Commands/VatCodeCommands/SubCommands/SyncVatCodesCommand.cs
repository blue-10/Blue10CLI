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

namespace Blue10CLI.Commands.VatCodeCommands
{
    public class SyncVatCodesCommand : Command
    {
        private readonly IVatCodeService _vatCodeService;
        private readonly IInOutService _utilities;
        private readonly ILogger<SyncVatCodesCommand> _logger;

        public SyncVatCodesCommand(IVatCodeService vatCodeService, IInOutService utilities, ILogger<SyncVatCodesCommand> logger) : base("sync",
            Descriptions.SyncVatCodeDescription)
        {
            _vatCodeService = vatCodeService;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<FileInfo?>(new[] { "-i", "--input" }, () => null, Descriptions.InputVatCodeDescription) { IsRequired = true });
            Add(new Option<EFormatType>(new[] { "--input-format" }, () => EFormatType.JSON, Descriptions.InputFormatDescription) { IsRequired = true });
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, Descriptions.OutputDescription));
            Add(new Option<EFormatType>(new[] { "-f", "--format", "--output-format" }, () => EFormatType.JSON, Descriptions.FormatDescription));

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

            var fVatCodes = _utilities.ReadAs<VatCode>(inputformat, fVatCodeList);
            if (fVatCodes is null)
                return;


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
                    _logger.LogWarning($"{fCount}/{fTotalVATCodes}: Failed syncing VATCode '{fVatCode.Name}' - {fResult.ErrorMessage}");
                }
                else
                {
                    fSuccessList.Add(fResult.Object);
                    Console.WriteLine($"{fCount}/{fTotalVATCodes} Successfully synced VATCode '{fVatCode.Name}'");
                }
                fCount++;
            }

            Console.WriteLine($"{fSuccessList.Count}/{fTotalVATCodes} VATCodes have been successfully imported");

            await _utilities.HandleOutput(outputformat, fSuccessList, output);
            if (output != null)
            {
                await _utilities.HandleOutputToFilePath(outputformat, fFailedList, $"{output?.Directory?.FullName}/failed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}");
                await _utilities.HandleOutputToFilePath(outputformat, fSuccessList, $"{output?.Directory?.FullName}/succeed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}");
            }
        }
    }
}