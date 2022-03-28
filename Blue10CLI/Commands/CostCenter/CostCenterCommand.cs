using System.CommandLine;

namespace Blue10CLI.Commands.CostCenterCommands
{
    public class CostCenterCommand : Command
    {
        public CostCenterCommand(
            ListCostCentersCommand listCostCenters,
            SyncCostCentersCommand syncCostCenters
            ) : base("CostCenter", "creates lists and manages Cost Units in the environments")
        {
            Add(listCostCenters);
            Add(syncCostCenters);
        }
    }
}