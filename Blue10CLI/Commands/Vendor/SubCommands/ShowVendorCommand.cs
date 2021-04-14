using Blue10CLI.Services.Interfaces;
using System.CommandLine;

namespace Blue10CLI.commands
{
    public class ShowVendorCommand : Command
    {
        public ShowVendorCommand(IVendorService vendorService) : base("show", "Shows a single vendor and it's details")
        {
        }
    }
}