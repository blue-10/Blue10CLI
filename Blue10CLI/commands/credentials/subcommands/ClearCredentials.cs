using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Blue10CLI.services;

namespace Blue10CLI.commands.credentials
{
    public class ClearCredentials : Command
    {
        private readonly CredentialsService _creds;

        public ClearCredentials(CredentialsService creds) : base("clear", "Clears current saved api key")
        {
            _creds = creds;
            
            Handler = CommandHandler.Create(action: ClearApiKey);
        }
        
        private void ClearApiKey()
        {
            _creds.RemoveCredentials();
            Console.WriteLine("API key Cleared");
        }
    }
}