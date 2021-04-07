using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class DeleteVendorCommand : Command
    {
        public DeleteVendorCommand(VendorService vendorService) : base("delete", "Deletes a vendor")
        {
        }
    }
}