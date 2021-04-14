namespace Blue10CLI
{
    public static class Descriptions
    {
        // General
        public const string OutputDescription = "Enter path to write output of this command to file. Default output is console only.";
        public const string FormatDescription = "Format of the output file. (JSON, XML...)";

        // Vendor.Sync
        public const string SyncDescription = "Sync vendors from a file to the Blue10 environment. AdministrationCode and CompanyId is required for each vendor. Updating existing vendors requires Id. Creating new vendors requires empty Id.";
        public const string InputDescription = "Path to the input file. File should have the same structure as the result of the 'vendor list' command.";
        public const string InputFormatDescription = "Format of the input file. (JSON, XML...)";

    }
}
