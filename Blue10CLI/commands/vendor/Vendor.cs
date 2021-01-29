using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class Vendor : Command
    {
        private readonly VendorService _service;

        public Vendor(VendorService service) : base("vendor", "creates lists and manages vendors in the environments")
        {
            _service = service;
            Add(new CreateVendor(service));
            Add(new ListVendors(service));
            Add(new ShowVendor(service));
            Add(new SyncVendors(service));
            Add(new DeleteVendor(service));
        }
    }

    public class CreateVendor : Command
    {
        public CreateVendor(VendorService vendorService) : base("create", "Creates new vendor in the system")
        {
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