using Blue10CLI.Services;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Blue10CLI.Commands.CredentialsCommands
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