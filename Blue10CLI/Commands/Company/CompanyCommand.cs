using System.CommandLine;

namespace Blue10CLI.Commands.CompanyCommands
{
    public class CompanyCommand : Command
    {
        public CompanyCommand(ListCompaniesCommand list) : base("company", "Manage companies")
        {
            Add(list);
        }
    }
}