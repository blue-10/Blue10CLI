using Blue10CLI.Services;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Blue10CLI.Commands.CredentialsCommands
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