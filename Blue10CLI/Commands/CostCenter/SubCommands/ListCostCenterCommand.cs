using Blue10CLI.Enums;
using Blue10CLI.Helpers;
using Blue10CLI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace Blue10CLI.Commands.CostCenterCommands
{
    public class ListCostCentersCommand : Command
    {
        private readonly ICostCenterService _CostCenterService;
        private readonly IInOutService _utilities;
        private readonly ILogger<ListCostCentersCommand> _logger;

        public ListCostCentersCommand(
            ICostCenterService CostCenterService,
            IInOutService utilities,
            ILogger<ListCostCentersCommand> logger) :
            base("list", "Lists all known CostCenters in a company")
        {
            _CostCenterService = CostCenterService;
            _utilities = utilities;
            _logger = logger;

            Add(new Option<string?>(new[] { "-c", "--company-id" }, () => null, "The company identifier under which the CostCenters exists") { IsRequired = true });
            Add(new Option<string?>(new[] { "-q", "--query" }, () => null, Descriptions.QueryDescription));
            Add(new Option<EFormatType>(new[] { "-f", "--format" }, () => EFormatType.JSON, Descriptions.FormatDescription));
            Add(new Option<FileInfo?>(new[] { "-o", "--output" }, () => null, Descriptions.OutputDescription));

            Handler = CommandHandler.Create<string, string, EFormatType, FileInfo?>(ListCostCentersHandler);
        }

        private async Task ListCostCentersHandler(string companyid, string? query, EFormatType format, FileInfo? output)
        {
            var resultObject = await _CostCenterService.List(companyid);
            await _utilities.HandleOutput(format, resultObject, output, query);
        }
    }
}