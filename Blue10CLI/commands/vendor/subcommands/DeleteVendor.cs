using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class DeleteVendor : Command
    {
        public DeleteVendor(VendorService vendorService) : base("delete", "Deletes a vendor")
        {
        }
    }
}