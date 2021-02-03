using System.CommandLine;

namespace Blue10CLI.commands.credentials
{
    public class Credentials : Command
    {
        public Credentials(SetCredentials set, ShowCredentials show, ClearCredentials clear,CheckCredentials check) : base("credentials", "Show and set API credentials")
        {
            Add(set);
            Add(show);
            Add(clear);
            Add(check);
        }
    }
}