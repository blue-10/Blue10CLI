using Blue10CLI.Services.Interfaces;
using System.CommandLine;

namespace Blue10CLI.Commands.VendorCommands
{
    public class DeleteVendorCommand : Command
    {
        public DeleteVendorCommand(IVendorService vendorService) : base("delete", "Deletes a vendor")
        {
        }
    }
}