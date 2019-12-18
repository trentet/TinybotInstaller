using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TinybotInstaller.Tasks;

namespace TinybotInstaller.Components
{
    static class ComponentTaskPresets
    {
        public static ComponentTask InstallWithChoco(
            Architectures architecture,
            string displayName,
            string pkgName,
            string searchMask,
            bool force)
        {
            Func<bool> isChocoInstalledAction =
               () => ChocoUtil.IsChocoInstalled();

            Func<bool> isInternetConnected =
                () => Network.TestInternetConnection();

            List<Func<bool>> prereqFuncs = new List<Func<bool>>() { isInternetConnected, isChocoInstalledAction };

            Action installProgramAction =
               () => ChocoUtil.ChocoUpgrade(pkgName, displayName, false, searchMask, ChocoUtil.DefaultArgs);

            Func<bool> isProgramInstalledAction =
               () => ProgramInstaller.IsSoftwareInstalled(architecture, searchMask);

            

            return new ComponentTask(new SetupTask(prereqFuncs, installProgramAction, isProgramInstalledAction, force), null);
        }

        public static ComponentTask PinProgramOrShortcutToTaskbar(string programOrShortcutFilePath)
        {
            // Check if the file exists and is an .exe or shortcut
            Func<bool> doesFileExist =
            () => 
            {
                return File.Exists(programOrShortcutFilePath);
            };

            Func<bool> isFileEXEorShortcutAction =
            () =>
            {
                return SystemUtil.IsLink(programOrShortcutFilePath)
                    || Path.GetExtension(programOrShortcutFilePath).Equals(".exe", StringComparison.InvariantCultureIgnoreCase);
            };

            Action<object> pinProgramOrShortcut =
            (object filePath) =>
            {
                string originalImagePathName = SystemUtil.GetImagePathName();
                try
                {
                    SystemUtil.ChangeImagePathName("explorer.exe");
                    SystemUtil.SetPin(ActionIndexes.PIN_TO_TASKBAR, (string)filePath);
                    
                }
                finally
                {
                    SystemUtil.ChangeImagePathName(originalImagePathName);
                }
            };

            // Check if the program or shortcut has been pinned to the taskbar
            Func<bool> isProgramOrShortcutPinned =
            () => 
            {
                return SystemUtil.IsFilePinned(programOrShortcutFilePath);
            };
            
            return new ComponentTask(
                new SetupTask(
                        new List<Func<bool>>() { doesFileExist, isFileEXEorShortcutAction }, 
                        pinProgramOrShortcut, 
                        programOrShortcutFilePath, 
                        isProgramOrShortcutPinned, 
                        false), 
                null);
        }

        public static ComponentTask SetRegistryRunCommand(
            bool runOnce,
            RegistryUtil.StartupRegistryHives startupRegistryHive,
            string registryKeyValueName,
            CLICommand command,
            StringComparison validationStringComparison
            )
        {
            /************************************************
             * Task Type: Setup
             * Task Prerequisite: None.
             * Task Description: Create a Run or RunOnce registry key 
             *                   at the HKLM or HKCU registry hive for 
             *                   a cmd /c command.
             * Task Validation: No cleanup required.
             ************************************************/

            Func<bool> prequisiteFunc = null;

            Action setupAction =
            () => RegistryUtil.SetRunCommand(
                    runOnce,
                    startupRegistryHive,
                    registryKeyValueName,
                    command);

            Func<bool> setupActionValidation =
            () => RegistryUtil.RegistryKeyValueDataExists(
                    RegistryUtil.GetStartupRunKey(runOnce, startupRegistryHive),
                    registryKeyValueName,
                    SystemPathConstants.CmdPathCommand + " " + command.ToString(),
                    validationStringComparison);

            SetupTask setupTask = new SetupTask(prequisiteFunc, setupAction, setupActionValidation, true);

            /************************************************/

            /************************************************
             * Task Type: Cleanup
             * Task Description: No cleanup required.
             ************************************************/
            Action cleanupAction = null;
            Func<bool> cleanupActionValidation = null;

            CleanupTask cleanupTask = new CleanupTask(cleanupAction, cleanupActionValidation);

            /************************************************/

            return new ComponentTask(setupTask, cleanupTask);
        }

        public static ComponentTask SetRegistryRunCommand(
            bool runOnce,
            RegistryUtil.StartupRegistryHives startupRegistryHive,
            string registryKeyValueName,
            CLICommand[] commands,
            StringComparison validationStringComparison
            )
        {
            /************************************************
             * Task Type: Setup
             * Task Prerequisite: None.
             * Task Description: Create a Run or RunOnce registry key 
             *                   at the HKLM or HKCU registry hive for 
             *                   a cmd /c command.
             * Task Validation: No cleanup required.
             ************************************************/

            Func<bool> prequisiteFunc = null;

            Action setupAction =
            () => RegistryUtil.SetRunCommand(
                    runOnce,
                    startupRegistryHive,
                    registryKeyValueName,
                    commands);

            Func<bool> setupActionValidation =
            () => RegistryUtil.RegistryKeyValueDataExists(
                    RegistryUtil.GetStartupRunKey(runOnce, startupRegistryHive),
                    registryKeyValueName,
                    SystemPathConstants.CmdPathCommand + " " + CMDCommands.ToOneLine(commands),
                    validationStringComparison);

            SetupTask setupTask = new SetupTask(prequisiteFunc, setupAction, setupActionValidation, true);

            /************************************************/

            /************************************************
             * Task Type: Cleanup
             * Task Description: No cleanup required.
             ************************************************/
            Action cleanupAction = null;
            Func<bool> cleanupActionValidation = null;

            CleanupTask cleanupTask = new CleanupTask(cleanupAction, cleanupActionValidation);

            /************************************************/

            return new ComponentTask(setupTask, cleanupTask);
        }

        public static ComponentTask SetRegistryRunCommand(
            bool runOnce,
            RegistryUtil.StartupRegistryHives startupRegistryHive,
            string registryKeyValueName,
            CMDForLoopCommand command,
            StringComparison validationStringComparison
            )
        {
            /************************************************
             * Task Type: Setup
             * Task Prerequisite: None.
             * Task Description: Create a Run or RunOnce registry key 
             *                   at the HKLM or HKCU registry hive for 
             *                   a cmd /c command.
             * Task Validation: No cleanup required.
             ************************************************/

            Func<bool> prequisiteFunc = null;

            Action setupAction =
            () => RegistryUtil.SetRunCommand(
                    runOnce,
                    startupRegistryHive,
                    registryKeyValueName,
                    command);

            Func<bool> setupActionValidation =
            () => RegistryUtil.RegistryKeyValueDataExists(
                    RegistryUtil.GetStartupRunKey(runOnce, startupRegistryHive),
                    registryKeyValueName,
                    SystemPathConstants.CmdPathCommand + " " + command.ToString(),
                    validationStringComparison);

            SetupTask setupTask = new SetupTask(prequisiteFunc, setupAction, setupActionValidation, true);

            /************************************************/

            /************************************************
             * Task Type: Cleanup
             * Task Description: No cleanup required.
             ************************************************/
            Action cleanupAction = null;
            Func<bool> cleanupActionValidation = null;

            CleanupTask cleanupTask = new CleanupTask(cleanupAction, cleanupActionValidation);

            /************************************************/

            return new ComponentTask(setupTask, cleanupTask);
        }
    }
}
