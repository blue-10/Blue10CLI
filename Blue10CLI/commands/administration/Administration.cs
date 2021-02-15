using System.CommandLine;

namespace Blue10CLI.commands.credentials
{
    public class Administration : Command
    {
        public Administration(CreateAdministration create, DeleteAdministration delete, ListAdministrations list,ShowAdministration show) : base("administration", "Manage administration(companies)")
        {
            Add(create);
            Add(show);
            Add(delete);
            Add(list);
        }
    }
}