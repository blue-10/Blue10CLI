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

namespace Blue10CLI.Commands.GLAccountCommands
{
    public class SyncGLAccountsCommand : Command
    {
        private readonly IGLAccountService _glaccountService;
        private readonly ILogger<SyncGLAccountsCommand> _logger;

        public SyncGLAccountsCommand(
            IGLAccountService glaccountService,
            ILogger<SyncGLAccountsCommand> logger) :
            base("sync",
                Descriptions.SyncGLAccountDescription)
        {
            _glaccountService = glaccountService;
            _logger = logger;

            Add(new Option<FileInfo?>(
                new[] { "-i", "--input" },
                () => null,
                Descriptions.InputGLAccountDescription)
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

            Handler = CommandHandler.Create<FileInfo, EFormatType, FileInfo?, EFormatType>(ImportGLAccountsHandler);
        }

        private async Task ImportGLAccountsHandler(
            FileInfo input,
            EFormatType inputformat,
            FileInfo? output,
            EFormatType outputformat)
        {
            var fSyncFilePath = input.FullName;
            var fGLAccountList = File.ReadAllText(fSyncFilePath);

            IList<GLAccount> fGLAccounts;

            try
            {
                fGLAccounts = inputformat.ReadAs<GLAccount>(fGLAccountList);
            }
            catch (ArgumentOutOfRangeException e)
            {
                _logger.LogError($"{outputformat} is not supported for this action: {e.Message}");
                throw;
            }
            catch (Exception ex) when (
                ex is JsonSerializationException
                || ex is CsvHelper.ReaderException
                || ex is InvalidOperationException)
            {
                _logger.LogError("Invalid input file. Check if format of the file is correct and if Id values of GLAccounts are valid");
                throw;
            }

            var fSuccessList = new List<GLAccount>();
            var fFailedList = new List<GLAccount>();

            var fCount = 1;
            var fTotalGLAccounts = fGLAccounts.Count;
            foreach (var fGLAccount in fGLAccounts)
            {
                var fResult = await _glaccountService.CreateOrUpdate(fGLAccount);
                if (fResult.Object == null)
                {
                    fFailedList.Add(fGLAccount);
                    _logger.LogWarning($"{fCount}/{fTotalGLAccounts}: Failed syncing GLAccount '{fGLAccount.Name}' - {fResult.ErrorMessage}");
                }
                else
                {
                    fSuccessList.Add(fResult.Object);
                    Console.WriteLine($"{fCount}/{fTotalGLAccounts} Successfully synced GLAccount '{fGLAccount.Name}'");
                }
                fCount++;
            }

            Console.WriteLine($"{fSuccessList.Count}/{fTotalGLAccounts} GLAccounts have been successfully imported");

            try
            {
                await outputformat.HandleOutput(fSuccessList, output);
                if (output != null)
                {
                    outputformat.HandleOutputToFilePath(fFailedList, $"{output?.Directory?.FullName}/failed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}").Wait();
                    outputformat.HandleOutputToFilePath(fSuccessList, $"{output?.Directory?.FullName}/succeed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}").Wait();
                }
            }
            catch (ArgumentOutOfRangeException e)
            {
                _logger.LogError($"{outputformat} is not supported for this action: {e.Message}");
            }
        }
    }
}