using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands.credentials
{
    public class Show : Command
    {
        private readonly CredentialsService _creds;

        public Show(CredentialsService creds) : base("show", "Shows current api key")
        {
            _creds = creds;
        }
    }
}