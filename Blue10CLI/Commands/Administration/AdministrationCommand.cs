using System.CommandLine;

namespace Blue10CLI.commands.credentials
{
    public class AdministrationCommand : Command
    {
        public AdministrationCommand(ListCompaniesCommand list) : base("administration", "Manage administration(companies)")
        {
            Add(list);
        }
    }
}