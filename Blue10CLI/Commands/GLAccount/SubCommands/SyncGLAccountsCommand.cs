using Blue10CLI.Services.Interfaces;
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
    public class SyncGLAccountsCommand : Command
    {
        private readonly IGLAccountService _glaccountService;

        public SyncGLAccountsCommand(IGLAccountService glaccountService) : base("sync",
            Descriptions.SyncGLAccountDescription)
        {
            _glaccountService = glaccountService;

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
                fGLAccounts = inputformat switch
                {
                    EFormatType.JSON => JsonConvert.DeserializeObject<IList<GLAccount>>(fGLAccountList),
                    EFormatType.CSV => Read.CsvRecords<GLAccount>(fGLAccountList, ","),
                    EFormatType.TSV => Read.CsvRecords<GLAccount>(fGLAccountList, "\t"),
                    EFormatType.SSV => Read.CsvRecords<GLAccount>(fGLAccountList, ";"),
                    EFormatType.XML => Read.XmlRecords<GLAccount>(fGLAccountList),
                    _ => throw new ArgumentOutOfRangeException(nameof(inputformat), inputformat, null)
                };
            }
            catch (Exception ex) when (
                ex is JsonSerializationException
                || ex is CsvHelper.ReaderException
                || ex is InvalidOperationException)
            {
                Console.WriteLine("Invalid input file. Check if format of the file is correct and if Id values of GLAccounts are valid");
                throw;
            }

            var fSuccessList = new List<GLAccount>();
            var fFailedList = new List<GLAccount>();

            var fCount = 1;
            var fTotalGLAccounts = fGLAccounts.Count;
            foreach (var fGLAccount in fGLAccounts)
            {
                var fResult = await _glaccountService.CreateOrUpdate(fGLAccount);
                if (fResult.GLAccount == null)
                {
                    fFailedList.Add(fGLAccount);
                    Console.WriteLine($"{fCount}/{fTotalGLAccounts}: Failed syncing GLAccount '{fGLAccount.Name}' - {fResult.ErrorMessage}");
                }
                else
                {
                    fSuccessList.Add(fResult.GLAccount);
                    Console.WriteLine($"{fCount}/{fTotalGLAccounts} Successfully synced GLAccount '{fGLAccount.Name}'");
                }
                fCount++;
            }

            outputformat.HandleOutput(fSuccessList, output).Wait();
            if (output != null)
            {
                outputformat.HandleOutputToFilePath(fFailedList, $"{output?.Directory?.FullName}/failed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}").Wait();
                outputformat.HandleOutputToFilePath(fSuccessList, $"{output?.Directory?.FullName}/succeed_{output?.Name ?? "NO_FILE_PATH_PROVIDED"}").Wait();
            }
            Console.WriteLine($"{fSuccessList.Count}/{fTotalGLAccounts} GLAccounts have been successfully imported");
        }
    }
}