using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class CreateAdministration : Command
    {
        public CreateAdministration(InvoiceService service) : base("create", "Creates new Administration (Company) in the target Blue10 Environment")
        {
            
        }
    }
}