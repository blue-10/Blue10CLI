using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands.credentials
{
    public class ClearCredentials : Command
    {
        private readonly CredentialsService _creds;

        public ClearCredentials(CredentialsService creds) : base("clear", "Clears current saved api key")
        {
            _creds = creds;
        }
    }
}