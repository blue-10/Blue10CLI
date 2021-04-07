using Blue10SDK.Models;

namespace Blue10CLI.models
{
    public class VendorResultModel
    {
        public VendorResultModel(Vendor? pVendor, string? pErrorMessage)
        {
            Vendor = pVendor;
            ErrorMessage = pErrorMessage;
        }

        public Vendor? Vendor { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
