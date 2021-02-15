using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class DeleteAdministration : Command
    {
        public DeleteAdministration(InvoiceService service) : base("delete", "Deletes an Administration (Company) in the target Blue10 Environment")
        {
        }
    }
}