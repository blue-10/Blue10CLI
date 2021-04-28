using Blue10CLI.Services.Interfaces;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Blue10CLI.Commands.CredentialsCommands
{
    public class ShowCredentialsCommand : Command
    {
        private readonly ICredentialsService _credentialService;

        public ShowCredentialsCommand(ICredentialsService credentialService) : base("show", "Shows current api key")
        {
            _credentialService = credentialService;
            Handler = CommandHandler.Create(ShowKey);
        }

        private void ShowKey()
        {
            Console.WriteLine(_credentialService.GetApiKey());
        }
    }
}