using System.CommandLine;

namespace Blue10CLI.commands
{
    public class VendorCommand : Command
    {
        public VendorCommand(
            CreateVendor createVendor,
            ListVendors listVendors,
            ShowVendor showVendor,
            SyncVendorsCommand syncVendors,
            DeleteVendor deleteVendor
            ) : base("vendor", "creates lists and manages vendors in the environments")
        {
            Add(createVendor);
            Add(listVendors);
            Add(syncVendors);

        }
    }
}