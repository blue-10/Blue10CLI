using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using Blue10CLI.services;

namespace Blue10CLI.commands.credentials
{
    public class SetCredentialsCommand : Command
    {
        private readonly CredentialsService _creds;

        public SetCredentialsCommand(CredentialsService creds) : base("set", "Sets new Api Key and overrides the old one")
        {
            _creds = creds;
            Handler = CommandHandler.Create<string>(SetApiKey);
        }

        private void SetApiKey(string obj)
        {            
            Console.WriteLine("Please insert your Blue10 API Key insert here:");
            _creds.SetApiKey(CredentialsService.ReadPassword());
        }
    }
}