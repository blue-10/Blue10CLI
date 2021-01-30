using System.CommandLine;
using Blue10CLI.commands;

namespace Blue10CLI
{
    public class Root : RootCommand
    {
        public Root(Vendor vendor, Invoice invoice)
        {
            Add(vendor);
            Add(invoice);
            Add(new Option<bool>("--debug", "Run command in debug mode to view detailed logs"));
        }
    }
}