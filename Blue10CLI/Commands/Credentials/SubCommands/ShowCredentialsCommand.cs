using Blue10CLI.services;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Blue10CLI.commands.credentials
{
    public class ShowCredentialsCommand : Command
    {
        private readonly CredentialsService _creds;

        public ShowCredentialsCommand(CredentialsService creds) : base("show", "Shows current api key")
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