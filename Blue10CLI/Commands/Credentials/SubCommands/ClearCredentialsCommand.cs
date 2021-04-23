using Blue10CLI.Services.Interfaces;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Blue10CLI.Commands.CredentialsCommands
{
    public class ClearCredentialsCommand : Command
    {
        private readonly ICredentialsService _credentialService;

        public ClearCredentialsCommand(ICredentialsService credentialService) : base("clear", "Clears current saved api key")
        {
            _credentialService = credentialService;

            Handler = CommandHandler.Create(action: ClearApiKey);
        }

        private void ClearApiKey()
        {
            _credentialService.RemoveCredentials();
            Console.WriteLine("API key Cleared");
        }
    }
}