using System.CommandLine;

namespace Blue10CLI.commands.credentials
{
    public class Credentials : Command
    {
        public Credentials(Set set, Show show, Clear clear) : base("credentials", "Show and set API credentials")
        {
            Add(set);
            Add(show);
            Add(clear);
        }
    }
}