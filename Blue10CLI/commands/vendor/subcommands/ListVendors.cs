using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class ListVendors : Command
    {
        public ListVendors(VendorService vendorService) : base("list", "Lists all known vendors in environment")
        {
        }
    }
}