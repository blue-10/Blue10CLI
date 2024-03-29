﻿using Blue10CLI.Commands.CompanyCommands;
using Blue10CLI.Commands.CredentialsCommands;
using Blue10CLI.Commands.GLAccountCommands;
using Blue10CLI.Commands.InvoiceCommands;
using Blue10CLI.Commands.VatCodeCommands;
using Blue10CLI.Commands.VendorCommands;
using System.CommandLine;

namespace Blue10CLI.Commands
{
    public class Root : RootCommand
    {
        public Root(
            VendorCommand vendor,
            InvoiceCommand invoice,
            GLAccountCommand glaccount,
            VatCodeCommand vatcode,
            CompanyCommand company,
            CredentialsCommand credentials)
        {
            Add(vendor);
            Add(invoice);
            Add(glaccount);
            Add(vatcode);
            Add(credentials);
            Add(company);
            Add(new Option<bool>("--debug", "Run command in debug mode to view detailed logs"));
        }
    }
}