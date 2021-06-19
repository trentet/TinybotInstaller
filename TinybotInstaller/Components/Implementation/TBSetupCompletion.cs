using System;
using System.IO;
using TinybotInstaller.Tasks;

namespace TinybotInstaller.Components
{
    class TBSetupCompletion : Component
    {
        public TBSetupCompletion() : base("Tinybot Installer Cleanup")
        {
            ComponentTasks.Add(1, SetRegistry_ScheduleTBInstallerDeletion());
        }

        private ComponentTask SetRegistry_ScheduleTBInstallerDeletion()
        {
            bool runOnce = true;
            RegistryUtil.StartupRegistryHives startupRegistryHive = RegistryUtil.StartupRegistryHives.LOCAL_MACHINE;
            string registryKeyValueName = "TBInstallerCleanup_DeleteInstaller";

            bool force = true;
            bool deleteFromSubfolders = false;
            bool quiet = true;
            string filePath = StringUtil.DQuote(Path.Combine(SystemPathConstants.StartupPath, "FirstSetup.exe"));
            
            CLICommand command =
                CMDCommands.Del(
                    force,
                    deleteFromSubfolders,
                    quiet,
                    filePath);

            StringComparison validationStringComparison = StringComparison.OrdinalIgnoreCase;

            return ComponentTaskPresets.SetRegistryRunCommand(
                runOnce,
                startupRegistryHive,
                registryKeyValueName,
                command,
                validationStringComparison);
        }

        private ComponentTask SetRegistry_ScheduleTBSetupFolderDeletion()
        {
            bool runOnce = true;
            RegistryUtil.StartupRegistryHives startupRegistryHive = RegistryUtil.StartupRegistryHives.LOCAL_MACHINE;
            string registryKeyValueName = "TBInstallerCleanup_SetupFolder";

            CLICommand command = new CLICommand();
            command.Program = "rmdir";
            command.Options.Add(new CommandOption("/", "s"));
            command.Options.Add(new CommandOption("/", "q"));
            command.Options.Add(new CommandOption(StringUtil.DQuote(SetupProperties.Setup)));

            StringComparison validationStringComparison = StringComparison.OrdinalIgnoreCase;

            return ComponentTaskPresets.SetRegistryRunCommand(
                runOnce,
                startupRegistryHive,
                registryKeyValueName,
                command,
                validationStringComparison);
        }

        private ComponentTask SetRegistry_ScheduleChocolateyLocalTempFolderCleanup()
        {
            bool runOnce = false;
            RegistryUtil.StartupRegistryHives startupRegistryHive = RegistryUtil.StartupRegistryHives.LOCAL_MACHINE;
            string registryKeyValueName = "TBInstallerCleanup_ChocolateyLocalTempFolder";

            CLICommand command = new CLICommand();
            command.Program = "rmdir";
            command.Options.Add(new CommandOption("/", "s"));
            command.Options.Add(new CommandOption("/", "q"));
            command.Options.Add(new CommandOption(StringUtil.DQuote(ChocoUtil.ChocolateyLocalTemp)));

            StringComparison validationStringComparison = StringComparison.OrdinalIgnoreCase;

            return ComponentTaskPresets.SetRegistryRunCommand(
                runOnce,
                startupRegistryHive,
                registryKeyValueName,
                command,
                validationStringComparison);
        }

        private ComponentTask SetRegistry_ScheduleTBInstallerExecutionFolderDeletion()
        {
            bool runOnce = true;
            RegistryUtil.StartupRegistryHives startupRegistryHive = RegistryUtil.StartupRegistryHives.LOCAL_MACHINE;
            string registryKeyValueName = "TBInstallerCleanup_ScriptFolder";

            CLICommand command = new CLICommand();
            command.Program = "rmdir";
            command.Options.Add(new CommandOption("/", "s"));
            command.Options.Add(new CommandOption("/", "q"));
            command.Options.Add(new CommandOption(StringUtil.DQuote(SetupProperties.ExecutionPath)));

            StringComparison validationStringComparison = StringComparison.OrdinalIgnoreCase;

            return ComponentTaskPresets.SetRegistryRunCommand(
                runOnce,
                startupRegistryHive,
                registryKeyValueName,
                command,
                validationStringComparison);
        }

        private ComponentTask SetRegistry_ScheduleLocalAppDataTempFolderFilesCleanup()
        {
            bool runOnce = true;
            RegistryUtil.StartupRegistryHives startupRegistryHive = RegistryUtil.StartupRegistryHives.LOCAL_MACHINE;
            string registryKeyValueName = "TBInstallerCleanup_LocalAppDataTempFolderFiles";

            bool force = true;
            bool deleteFromSubfolders = true;
            bool quiet = true;
            string filePath = StringUtil.DQuote(@"C:\Users\Administrator\AppData\Local\Temp\*");

            CLICommand command =
                CMDCommands.Del(
                    force,
                    deleteFromSubfolders,
                    quiet,
                    filePath);

            StringComparison validationStringComparison = StringComparison.OrdinalIgnoreCase;

            return ComponentTaskPresets.SetRegistryRunCommand(
                runOnce,
                startupRegistryHive,
                registryKeyValueName,
                command,
                validationStringComparison);
        }

        private ComponentTask SetRegistry_ScheduleWindowsTempFolderFilesCleanup()
        {
            bool runOnce = true;
            RegistryUtil.StartupRegistryHives startupRegistryHive = RegistryUtil.StartupRegistryHives.LOCAL_MACHINE;
            string registryKeyValueName = "TBInstallerCleanup_WindowsTempFolderFiles";

            bool force = true;
            bool deleteFromSubfolders = true;
            bool quiet = true;
            string filePath = StringUtil.DQuote(@"C:\Windows\Temp\*");

            CLICommand command =
                CMDCommands.Del(
                    force,
                    deleteFromSubfolders,
                    quiet,
                    filePath);

            StringComparison validationStringComparison = StringComparison.OrdinalIgnoreCase;

            return ComponentTaskPresets.SetRegistryRunCommand(
                runOnce,
                startupRegistryHive,
                registryKeyValueName,
                command,
                validationStringComparison);
        }



        private ComponentTask SetRegistry_ScheduleLocalAppDataTempFolderFoldersCleanup()
        {
            bool runOnce = true;
            RegistryUtil.StartupRegistryHives startupRegistryHive = RegistryUtil.StartupRegistryHives.LOCAL_MACHINE;
            string registryKeyValueName = "TBInstallerCleanup_LocalAppDataTempFolderFolders";

            CMDForLoopCommand command = new CMDForLoopCommand();
            command.ForSwitch = new CommandOption("/", "d");
            command.LoopParameter = "%x";
            command.InParameter = StringUtil.DQuote(@"C:\Users\Administrator\AppData\Local\Temp\*");
            
            CLICommand loopedCommand = new CLICommand();
            loopedCommand.Program = "@rd";
            loopedCommand.Options.Add(new CommandOption("/", "s"));
            loopedCommand.Options.Add(new CommandOption("/", "q"));
            loopedCommand.Options.Add(new CommandOption(StringUtil.DQuote("%x")));
            
            command.LoopedCommand = loopedCommand;

            StringComparison validationStringComparison = StringComparison.OrdinalIgnoreCase;

            return ComponentTaskPresets.SetRegistryRunCommand(
                runOnce,
                startupRegistryHive,
                registryKeyValueName,
                command,
                validationStringComparison);
        }

        private ComponentTask SetRegistry_ScheduleWindowsTempFolderFoldersCleanup()
        {            
            bool runOnce = true;
            RegistryUtil.StartupRegistryHives startupRegistryHive = RegistryUtil.StartupRegistryHives.LOCAL_MACHINE;
            string registryKeyValueName = "TBInstallerCleanup_WindowsTempFolderFolders";

            CMDForLoopCommand command = new CMDForLoopCommand();
            command.ForSwitch = new CommandOption("/", "d");
            command.LoopParameter = "%x";
            command.InParameter = StringUtil.DQuote(@"C:\Windows\Temp\*");

            CLICommand loopedCommand = new CLICommand();
            loopedCommand.Program = "@rd";
            loopedCommand.Options.Add(new CommandOption("/", "s"));
            loopedCommand.Options.Add(new CommandOption("/", "q"));
            loopedCommand.Options.Add(new CommandOption(StringUtil.DQuote("%x")));

            command.LoopedCommand = loopedCommand;

            StringComparison validationStringComparison = StringComparison.OrdinalIgnoreCase;

            return ComponentTaskPresets.SetRegistryRunCommand(
                runOnce,
                startupRegistryHive,
                registryKeyValueName,
                command,
                validationStringComparison);
        }

        private ComponentTask SetRegistry_ScheduleResolutionSetter()
        {
            bool runOnce = true;
            RegistryUtil.StartupRegistryHives startupRegistryHive = RegistryUtil.StartupRegistryHives.CURRENT_USER;
            string registryKeyValueName = "TBInstaller_SetResolution";

            CLICommand sleepCommand = new CLICommand
            {
                Program = "sleep"
            };
            sleepCommand.Options.Add(new CommandOption("3"));

            CLICommand qresCommand = CMDCommands.QRes(SetupProperties.DisplayWidth, SetupProperties.DisplayHeight);

            CLICommand taskkillCommand = CMDCommands.TaskKill("explorer.exe");

            CLICommand startCommand = CMDCommands.Start("explorer.exe");

            CLICommand[] commands = new CLICommand[] { sleepCommand, qresCommand, taskkillCommand, startCommand };

            StringComparison validationStringComparison = StringComparison.OrdinalIgnoreCase;

            return ComponentTaskPresets.SetRegistryRunCommand(
                runOnce,
                startupRegistryHive,
                registryKeyValueName,
                commands,
                validationStringComparison);
        }
    }
}
