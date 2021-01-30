using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands.credentials
{
    public class Clear : Command
    {
        private readonly CredentialsService _creds;

        public Clear(CredentialsService creds) : base("clear", "Clears current saved api key")
        {
            _creds = creds;
        }
    }
}