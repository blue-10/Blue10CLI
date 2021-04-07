using Blue10CLI.commands;
using Blue10CLI.commands.credentials;
using System.CommandLine;

namespace Blue10CLI
{
    public class Root : RootCommand
    {
        public Root(VendorCommand vendor, InvoiceCommand invoice, AdministrationCommand administratration
            , CredentialsCommand credentials)
        {
            Add(vendor);
            Add(invoice);
            Add(credentials);
            Add(administratration);
            Add(new Option<bool>("--debug", "Run command in debug mode to view detailed logs"));
        }
    }
}