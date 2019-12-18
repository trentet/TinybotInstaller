using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinybotInstaller
{
    public class CLICommand
    {
        private string program;
        private List<CommandOption> options = new List<CommandOption>();
        private List<string> arguments;

        public string Program { get; set; }
        public List<CommandOption> Options { get; }
        public List<string> Arguments { get; }

        public CLICommand()
        {
            this.Program = "";
            this.Options = new List<CommandOption>();
            this.Arguments = new List<string>();
        }

        public CLICommand(string program)
        {
            this.Program = program;
            this.Options = new List<CommandOption>();
            this.Arguments = new List<string>();
        }

        public CLICommand(string program, List<CommandOption> options)
        {
            this.Program = program;
            this.Options = options;
            this.Arguments = new List<string>();
        }

        public CLICommand(string program, params string[] arguments)
        {
            this.Program = program;
            this.Options = new List<CommandOption>();
            this.Arguments = arguments.ToList();
        }

        public CLICommand(string program, List<CommandOption> options, params string[] arguments)
        {
            this.Program = program;
            this.Options = options;
            this.Arguments = arguments.ToList();
        }

        public override string ToString()
        {
            string fullCommand = Program;

            if (Options != null)
            {
                foreach (CommandOption commandSwitch in Options)
                {
                    if (commandSwitch.Prefix.Length > 0)
                    {
                        fullCommand += " " + commandSwitch.Prefix;
                    }

                    if (commandSwitch.Prefix.Length > 0)
                    {
                        fullCommand += " " + commandSwitch.Variable;
                    }

                    foreach (string arg in commandSwitch.Args)
                    {
                        fullCommand += " " + arg;
                    }
                }
            }

            return fullCommand;
        }
    }
}
