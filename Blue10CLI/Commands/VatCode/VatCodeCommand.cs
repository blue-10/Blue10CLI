using System.CommandLine;

namespace Blue10CLI.commands
{
    public class VatCodeCommand : Command
    {
        public VatCodeCommand(
            ListVatCodesCommand listVATCodes,
            SyncVatCodesCommand syncVATCodes
            ) : base("vatcode", "creates lists and manages VatCodes in the environments")
        {
            Add(listVATCodes);
            Add(syncVATCodes);

        }
    }
}