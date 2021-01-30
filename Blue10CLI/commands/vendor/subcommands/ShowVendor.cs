using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands
{
    public class ShowVendor : Command
    {
        public ShowVendor(VendorService vendorService) : base("show", "Shows a single vendor and it's details")
        {
        }
    }
}