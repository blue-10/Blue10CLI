using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands.credentials
{
    public class SetCredentials : Command
    {
        private readonly CredentialsService _creds;

        public SetCredentials(CredentialsService creds) : base("set", "Sets new Api Key and overrides the old one")
        {
            _creds = creds;
        }
    }
}