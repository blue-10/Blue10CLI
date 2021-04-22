using Blue10CLI.Services;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace Blue10CLI.Commands.CredentialsCommands
{
    public class ClearCredentialsCommand : Command
    {
        private readonly CredentialsService _creds;

        public ClearCredentialsCommand(CredentialsService creds) : base("clear", "Clears current saved api key")
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