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

            Action installProgramAction =
               () => ChocoUtil.ChocoUpgrade(pkgName, displayName, false, searchMask, ChocoUtil.DefaultArgs);

            Func<bool> isProgramInstalledAction =
               () => ProgramInstaller.IsSoftwareInstalled(architecture, searchMask);

            return new ComponentTask(new SetupTask(isChocoInstalledAction, installProgramAction, isProgramInstalledAction, force), null);
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
    }
}
