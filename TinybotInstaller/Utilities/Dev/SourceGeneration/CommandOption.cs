namespace TinybotInstaller
{
    public class CommandOption
    {
        private string prefix;
        private string variable;
        private string[] args;

        public string Prefix { get => prefix; set => prefix = value; }
        public string Variable { get => variable; set => variable = value; }
        public string[] Args { get => args; set => args = value; }

        public CommandOption(string variable, string[] args)
        {
            this.Prefix = null;
            this.Variable = variable;
            this.Args = args;
        }

        public CommandOption(string prefix, string variable, string[] args)
        {
            this.Prefix = prefix;
            this.Variable = variable;
            this.Args = args;
        }

        public CommandOption(string prefix, string variable)
        {
            this.Prefix = prefix;
            this.Variable = variable;
            this.Args = null;
        }

        public CommandOption(string[] args)
        {
            this.Prefix = null;
            this.Variable = null;
            this.Args = args;
        }

        public CommandOption(string arg)
        {
            this.Prefix = null;
            this.Variable = null;
            this.Args = new string[] { arg };
        }

        public override string ToString()
        {
            string commandSwitchToString = "";
            if (Prefix != null) {
                commandSwitchToString += Prefix;
            }

            if (Variable != null)
            {
                commandSwitchToString += Variable;
            }

            if (Args != null)
            {
                foreach(string arg in Args)
                {
                    if(commandSwitchToString.Trim().Length > 0)
                    {
                        commandSwitchToString += " ";
                    }

                    commandSwitchToString += arg;
                }
            }

            return commandSwitchToString;
        }
    }
}
