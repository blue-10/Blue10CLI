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

namespace Blue10CLI.Commands.CostUnitCommands
{
    public class SyncCostUnitsCommand : Command
    {
        private readonly ICostUnitService _CostUnitService;
        private readonly IInOutService _utilities;
        private readonly ILogger<SyncCostUnitsCommand> _logger;

        public SyncCostUnitsCommand(
            ICostUnitService CostUnitService,
            IInOutService utilities,
            ILogger<SyncCostUnitsCommand> logger) :
            base("sync",
                Descriptions.SyncGLAccountDescription)
        {
            _CostUnitService = CostUnitService;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<FileInfo?>(new[] { "-i", "--input" }, () => null, Descriptions.InputCostUnitDescription) { IsRequired = true });
            Add(new Option<EFormatType>(new[] { "--input-format" }, () => EFormatType.JSON, Descriptions.InputFormatDescription) { IsRequired = true });
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, Descriptions.OutputDescription));
            Add(new Option<EFormatType>(new[] { "-f", "--format", "--output-format" }, () => EFormatType.JSON, Descriptions.FormatDescription));

            Handler = CommandHandler.Create<FileInfo, EFormatType, FileInfo?, EFormatType>(ImportCostUnitsHandler);
        }

        private async Task ImportCostUnitsHandler(
            FileInfo input,
            EFormatType inputformat,
            FileInfo? output,
            EFormatType outputformat)
        {
            var fSyncFilePath = input.FullName;
            var fCostUnitList = File.ReadAllText(fSyncFilePath);

            var fCostUnits = _utilities.ReadAs<CostUnit>(inputformat, fCostUnitList);
            if (fCostUnits is null)
                return;

            var fSuccessList = new List<CostUnit>();
            var fFailedList = new List<CostUnit>();

            var fCount = 1;
            var fTotalCostUnits = fCostUnits.Count;
            foreach (var fCostUnit in fCostUnits)
            {
                var fResult = await _CostUnitService.CreateOrUpdate(fCostUnit);
                if (fResult.Object == null)
                {
                    fFailedList.Add(fCostUnit);
                    _logger.LogWarning($"{fCount}/{fTotalCostUnits}: Failed syncing CostUnit '{fCostUnit.Name}' - {fResult.ErrorMessage}");
                }
                else
                {
                    fSuccessList.Add(fResult.Object);
                    Console.WriteLine($"{fCount}/{fTotalCostUnits} Successfully synced CostUnit '{fCostUnit.Name}'");
                }
                fCount++;
            }

            Console.WriteLine($"{fSuccessList.Count}/{fTotalCostUnits} CostUnits have been successfully imported");

            await _utilities.HandleOutput(outputformat, fSuccessList, output);
            if (output != null)
            {
                await _utilities.HandleOutputToFilePath(outputformat, fFailedList, $"{output?.Directory?.FullName}/failed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}");
                await _utilities.HandleOutputToFilePath(outputformat, fSuccessList, $"{output?.Directory?.FullName}/succeed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}");
            }
        }
    }
}