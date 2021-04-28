using Blue10CLI.Services.Interfaces;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Blue10CLI.Commands.CredentialsCommands
{
    public class SetCredentialsCommand : Command
    {
        private readonly ICredentialsService _credentialService;

        public SetCredentialsCommand(ICredentialsService credentialService) : base("set", "Sets new Api Key and overrides the old one")
        {
            _credentialService = credentialService;
            Handler = CommandHandler.Create<string>(SetApiKey);
        }

        private void SetApiKey(string obj)
        {
            _credentialService.SetApiKey(_credentialService.ReadPassword());
        }
    }
}