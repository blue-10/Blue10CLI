using Blue10CLI.Enums;
using Blue10CLI.Helpers;
using Blue10CLI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.Commands.CostUnitCommands
{
    public class ListCostUnitsCommand : Command
    {
        private readonly ICostUnitService _CostUnitService;
        private readonly IInOutService _utilities;
        private readonly ILogger<ListCostUnitsCommand> _logger;

        public ListCostUnitsCommand(
            ICostUnitService CostUnitService,
            IInOutService utilities,
            ILogger<ListCostUnitsCommand> logger) :
            base("list", "Lists all known CostUnits in a company")
        {
            _CostUnitService = CostUnitService;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<string?>(new[] { "-c", "--company-id" }, () => null, "The company identifier under which the CostUnits exists") { IsRequired = true });
            Add(new Option<string?>(new[] { "-q", "--query" }, () => null, Descriptions.QueryDescription));
            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, Descriptions.FormatDescription));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, Descriptions.OutputDescription));

            Handler = CommandHandler.Create<string, string, EFormatType, FileInfo?>(ListCostUnitsHandler);
        }

        private async Task ListCostUnitsHandler(string companyid, string? query, EFormatType format, FileInfo? output)
        {
            var resultObject = await _CostUnitService.List(companyid);
            await _utilities.HandleOutput(format, resultObject, output, query);
        }
    }
}