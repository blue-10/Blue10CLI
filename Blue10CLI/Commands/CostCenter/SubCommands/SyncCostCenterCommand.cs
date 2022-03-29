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

namespace Blue10CLI.Commands.CostCenterCommands
{
    public class SyncCostCentersCommand : Command
    {
        private readonly ICostCenterService _CostCenterService;
        private readonly IInOutService _utilities;
        private readonly ILogger<SyncCostCentersCommand> _logger;

        public SyncCostCentersCommand(
            ICostCenterService CostCenterService,
            IInOutService utilities,
            ILogger<SyncCostCentersCommand> logger) :
            base("sync",
                Descriptions.SyncCostCenterDescription)
        {
            _CostCenterService = CostCenterService;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<FileInfo?>(new[] { "-i", "--input" }, () => null, Descriptions.InputCostCenterDescription) { IsRequired = true });
            Add(new Option<EFormatType>(new[] { "--input-format" }, () => EFormatType.JSON, Descriptions.InputFormatDescription) { IsRequired = true });
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, Descriptions.OutputDescription));
            Add(new Option<EFormatType>(new[] { "-f", "--format", "--output-format" }, () => EFormatType.JSON, Descriptions.FormatDescription));

            Handler = CommandHandler.Create<FileInfo, EFormatType, FileInfo?, EFormatType>(ImportCostCentersHandler);
        }

        private async Task ImportCostCentersHandler(
            FileInfo input,
            EFormatType inputformat,
            FileInfo? output,
            EFormatType outputformat)
        {
            var fSyncFilePath = input.FullName;
            var fCostCenterList = File.ReadAllText(fSyncFilePath);

            var fCostCenters = _utilities.ReadAs<CostCenter>(inputformat, fCostCenterList);
            if (fCostCenters is null)
                return;

            var fSuccessList = new List<CostCenter>();
            var fFailedList = new List<CostCenter>();

            var fCount = 1;
            var fTotalCostCenters = fCostCenters.Count;
            foreach (var fCostCenter in fCostCenters)
            {
                var fResult = await _CostCenterService.CreateOrUpdate(fCostCenter);
                if (fResult.Object == null)
                {
                    fFailedList.Add(fCostCenter);
                    _logger.LogWarning($"{fCount}/{fTotalCostCenters}: Failed syncing CostCenter '{fCostCenter.Name}' - {fResult.ErrorMessage}");
                }
                else
                {
                    fSuccessList.Add(fResult.Object);
                    Console.WriteLine($"{fCount}/{fTotalCostCenters} Successfully synced CostCenter '{fCostCenter.Name}'");
                }
                fCount++;
            }

            Console.WriteLine($"{fSuccessList.Count}/{fTotalCostCenters} CostCenters have been successfully imported");

            await _utilities.HandleOutput(outputformat, fSuccessList, output);
            if (output != null)
            {
                await _utilities.HandleOutputToFilePath(outputformat, fFailedList, $"{output?.Directory?.FullName}/failed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}");
                await _utilities.HandleOutputToFilePath(outputformat, fSuccessList, $"{output?.Directory?.FullName}/succeed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}");
            }
        }
    }
}