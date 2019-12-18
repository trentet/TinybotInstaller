using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinybotInstaller
{
    public static class CMDCommands
    {
        public enum DelOption
        {
            P_PROMPT = 0,
            F_FORCE = 1,
            S_RECURSE_SUBFOLDERS = 2,
            Q_QUIET = 3,
            A_FILE_ATTRIBUTES = 4
        }

        public static CLICommand TaskKill(string imageName)
        {
            //Requires update for all scenarios
            CLICommand taskkillCommand = new CLICommand("taskkill");
            taskkillCommand.Options.Add(new CommandOption("/", "F"));
            taskkillCommand.Options.Add(new CommandOption("/", "IM", new string[] { imageName }));

            return taskkillCommand;
        }

        public static CLICommand Del(bool force, bool deleteFromSubfolders, bool quiet, string filePath)
        {
            CLICommand delCommand = new CLICommand("del", filePath);

            if (force)
            {
                delCommand.Options.Add(new CommandOption("/", "F"));
            }

            if (deleteFromSubfolders)
            {
                delCommand.Options.Add(new CommandOption("/", "S"));
            }

            if (quiet)
            {
                delCommand.Options.Add(new CommandOption("/", "Q"));
            }
            else
            {
                delCommand.Options.Add(new CommandOption("/", "P"));
            }

            return delCommand;
        }

        public static CLICommand Sleep()
        {
            return null;
        }

        public static CLICommand QRes(int displayWidth, int displayHeight)
        {
            CLICommand qresCommand = new CLICommand("QRes.exe");
            qresCommand.Options.Add(new CommandOption("x:", new string[] { displayWidth + "" }));
            qresCommand.Options.Add(new CommandOption("y:", new string[] { displayHeight + "" }));

            return qresCommand;
        }

        public static CLICommand Start(string command)
        {
            CLICommand startCommand = new CLICommand("start");
            startCommand.Arguments.Add(command);

            return startCommand;
        }

        public static string ToOneLine(CLICommand[] commands)
        {
            if (commands != null)
            {
                if (commands.Length > 0)
                {
                    string oneLine = "";

                    foreach (CLICommand command in commands)
                    {
                        oneLine += command.ToString() + " && ";
                    }

                    if (oneLine.EndsWith(" && "))
                    {
                        oneLine = oneLine.Substring(0, oneLine.LastIndexOf(" && "));
                    }

                    return oneLine;
                }
                else
                {
                    throw new Exception("CLICommand[] " + nameof(commands) + " cannot be empty! ");
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(commands), "CLICommand[] " + nameof(commands) + " cannot be null! ");
            }
        }
    }
}
