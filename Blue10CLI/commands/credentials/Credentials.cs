using System.CommandLine;
using Blue10CLI.services;

namespace Blue10CLI.commands.credentials
{
    public class Credentials : Command
    {
        public Credentials(Set set, Show show, Clear clear) : base("credentials", "Show and set API credentials")
        {
            Add(set);
            Add(show);
            Add(clear);
        }
    }
    
    public class Set : Command
    {
        private readonly CredentialsService _creds;

        public Set(CredentialsService creds) : base("set", "Sets new Api Key and overrides the old one")
        {
            _creds = creds;
        }
    }
    
    public class Show : Command
    {
        private readonly CredentialsService _creds;

        public Show(CredentialsService creds) : base("show", "Shows current api key")
        {
            _creds = creds;
        }
    }
    
    public class Clear : Command
    {
        private readonly CredentialsService _creds;

        public Clear(CredentialsService creds) : base("clear", "Clears current saved api key")
        {
            _creds = creds;
        }
    }
}