﻿using System.CommandLine;

namespace Blue10CLI.Commands.GLAccountCommands
{
    public class GLAccountCommand : Command
    {
        public GLAccountCommand(
            ListGLAccountsCommand listGLAccounts,
            SyncGLAccountsCommand syncGLAccounts
            ) : base("glaccount", "creates lists and manages GLAccounts in the environments")
        {
            Add(listGLAccounts);
            Add(syncGLAccounts);
        }
    }
}