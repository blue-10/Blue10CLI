﻿namespace Blue10CLI
{
    public static class Descriptions
    {
        // General
        public const string OutputDescription = "Enter path to write output of this command to file. Default output is console only.";
        public const string FormatDescription = "Format of the output file. (JSON, XML...)";
        public const string QueryDescription = "A query used to filter out results. NOTE: Dependant on output format. If output is 'json', this is a JMESPath query to filter results. https://jmespath.org/. If output is 'xml', this is an XPATH string. https://www.w3schools.com/xml/xpath_intro.asp";
        public const string InputFormatDescription = "Format of the input file. (JSON, XML...)";

        // Vendor.Sync
        public const string SyncVendorDescription = "Sync vendors from a file to the Blue10 environment. AdministrationCode and CompanyId is required for each vendor. Updating existing vendors requires Id. Creating new vendors requires empty Id.";
        public const string InputVendorDescription = "Path to the input file. File should have the same structure as the result of the 'vendor list' command.";

        // GLAccount.Sync
        public const string SyncGLAccountDescription = "Sync GLAccounts from a file to the Blue10 environment. AdministrationCode and CompanyId is required for each GLAccount. Updating existing GLAccounts requires Id. Creating new GLAccounts requires empty Id.";
        public const string InputGLAccountDescription = "Path to the input file. File should have the same structure as the result of the 'glaccount list' command.";
    }
}
