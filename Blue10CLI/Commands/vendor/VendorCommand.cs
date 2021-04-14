using System.CommandLine;

namespace Blue10CLI.commands
{
    public class VendorCommand : Command
    {
        public VendorCommand(
            CreateVendorCommand createVendor,
            ListVendorsCommand listVendors,
            ShowVendorCommand showVendor,
            SyncVendorsCommand syncVendors,
            DeleteVendorCommand deleteVendor
            ) : base("vendor", "creates lists and manages vendors in the environments")
        {
            Add(createVendor);
            Add(listVendors);
            Add(syncVendors);

        }
    }
}