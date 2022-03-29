using System.CommandLine;

namespace Blue10CLI.Commands.CostUnitCommands
{
    public class CostUnitCommand : Command
    {
        public CostUnitCommand(
            ListCostUnitsCommand listCostUnits,
            SyncCostUnitsCommand syncCostUnits
            ) : base("CostUnit", "creates lists and manages Cost Units in the environments")
        {
            Add(listCostUnits);
            Add(syncCostUnits);
        }
    }
}