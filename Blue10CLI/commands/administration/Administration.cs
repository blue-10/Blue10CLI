using System.CommandLine;

namespace Blue10CLI.commands.credentials
{
    public class Administration : Command
    {
        public Administration(ListCompanies list) : base("administration", "Manage administration(companies)")
        {
            Add(list);
        }
    }
}