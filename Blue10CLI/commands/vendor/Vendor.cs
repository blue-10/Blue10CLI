using System.CommandLine;

namespace Blue10CLI.commands
{
    public class Vendor : Command
    {
        public Vendor(
            CreateVendor createVendor,
            ListVendors listVendors,
            ShowVendor showVendor,
            SyncVendors syncVendors,
            DeleteVendor deleteVendor
            ) : base("vendor", "creates lists and manages vendors in the environments")
        {
            Add(createVendor);
            Add(listVendors);
            //Add(showVendor);
            //Add(syncVendors);
            //Add(deleteVendor);
        }
    }
}