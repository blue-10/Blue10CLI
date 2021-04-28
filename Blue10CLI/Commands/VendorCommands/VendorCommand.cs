using System.CommandLine;

namespace Blue10CLI.Commands.VendorCommands
{
    public class VendorCommand : Command
    {
        public VendorCommand(
            CreateVendorCommand createVendor,
            ListVendorsCommand listVendors,
            SyncVendorsCommand syncVendors
            ) : base("vendor", "creates lists and manages vendors in the environments")
        {
            Add(createVendor);
            Add(listVendors);
            Add(syncVendors);

        }
    }
}