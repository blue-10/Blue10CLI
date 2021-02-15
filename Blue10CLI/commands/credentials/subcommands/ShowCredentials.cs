using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Blue10CLI.services;

namespace Blue10CLI.commands.credentials
{
    public class ShowCredentials : Command
    {
        private readonly CredentialsService _creds;

        public ShowCredentials(CredentialsService creds) : base("show", "Shows current api key")
        {
            _creds = creds;
            Handler = CommandHandler.Create(ShowKey);
            
        }

        private void ShowKey()
        {
            Console.WriteLine(_creds.GetApiKey());
        }
    }
}