using Blue10SDK.Models;

namespace Blue10CLI.models
{
    public class GLAccountResultModel
    {
        public GLAccountResultModel(GLAccount? pGLAccount, string? pErrorMessage)
        {
            GLAccount = pGLAccount;
            ErrorMessage = pErrorMessage;
        }

        public GLAccount? GLAccount { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
