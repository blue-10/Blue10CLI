using System.CommandLine;
using System.CommandLine.Invocation;
using Blue10CLI.services;

namespace Blue10CLI.commands.credentials
{
    public class SetCredentials : Command
    {
        private readonly CredentialsService _creds;

        public SetCredentials(CredentialsService creds) : base("set", "Sets new Api Key and overrides the old one")
        {
            _creds = creds;
            Handler = CommandHandler.Create<string>(SetApiKey);
        }

        private void SetApiKey(string obj)
        {
            _creds.SetApiKey(CredentialsService.ReadPassword());
        }
    }
}