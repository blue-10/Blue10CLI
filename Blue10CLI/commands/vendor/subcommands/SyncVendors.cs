using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class SyncVendors : Command
    {
        public SyncVendors(VendorService vendorService) : base("sync", "Synchronises all vendors with a provided file")
        {
        }
    }
}