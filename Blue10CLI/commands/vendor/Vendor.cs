using System.CommandLine;
using Blue10CLI.services;

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
            Add(showVendor);
            Add(syncVendors);
            Add(deleteVendor);
        }
    }

    public class CreateVendor : Command
    {
        public CreateVendor(VendorService vendorService) : base("create", "Creates new vendor in the system")
        {
            vendorService.Create();
        }
    }

    public class ListVendors : Command
    {
        public ListVendors(VendorService vendorService) : base("list", "Lists all known vendors in environment")
        {
        }
    }

    public class ShowVendor : Command
    {
        public ShowVendor(VendorService vendorService) : base("show", "Shows a single vendor and it's details")
        {
        }
    }

    public class SyncVendors : Command
    {
        public SyncVendors(VendorService vendorService) : base("sync", "Synchronises all vendors with a provided file")
        {
        }
    }

    public class DeleteVendor : Command
    {
        public DeleteVendor(VendorService vendorService) : base("delete", "Deletes a vendor")
        {
        }
    }
}