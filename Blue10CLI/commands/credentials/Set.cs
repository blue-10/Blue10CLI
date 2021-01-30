using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands.credentials
{
    public class Set : Command
    {
        private readonly CredentialsService _creds;

        public Set(CredentialsService creds) : base("set", "Sets new Api Key and overrides the old one")
        {
            _creds = creds;
        }
    }
}