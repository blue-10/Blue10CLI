﻿using System.CommandLine;

namespace Blue10CLI.Commands.CredentialsCommands
{
    public class CredentialsCommand : Command
    {
        public CredentialsCommand(SetCredentialsCommand set, ShowCredentialsCommand show, ClearCredentialsCommand clear, CheckCredentialsCommand check) : base("credentials", "Show and set API credentials")
        {
            Add(set);
            Add(show);
            Add(clear);
            Add(check);
        }
    }
}