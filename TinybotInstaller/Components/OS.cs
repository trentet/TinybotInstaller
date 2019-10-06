using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinybotInstaller.Tasks;
using PowerManagerAPI;

namespace TinybotInstaller.Components
{
    class OS : Component
    {
        public OS() : base("OS Configuration")
        {
            ComponentTasks.Add(1, ConfigurePowerOptions());
            ComponentTasks.Add(2, ApplyContextMenuModifyingRegistryEdits());
            ComponentTasks.Add(3, ApplyPerformanceImprovingRegistryEdits());
            ComponentTasks.Add(4, CopyWindowsFiles());
            ComponentTasks.Add(5, ScheduleStartupTasks());
        }

        private ComponentTask ConfigurePowerOptions()
        {
            Func<bool> prequisiteFunc = null;

            Action action = 
            () => 
            {
                //SUB_DISK - DISKIDLE
                PowerManager.SetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.DISK_SUBGROUP, Setting.DISKIDLE, PowerMode.AC, 0);
                PowerManager.SetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.DISK_SUBGROUP, Setting.DISKIDLE, PowerMode.DC, 0);

                //SUB_SLEEP - STANDBYIDLE
                PowerManager.SetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.STANDBYIDLE, PowerMode.AC, 0);
                PowerManager.SetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.STANDBYIDLE, PowerMode.DC, 0);
                //SUB_SLEEP - HYBRIDSLEEP
                PowerManager.SetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.HYBRIDSLEEP, PowerMode.AC, 0);
                PowerManager.SetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.HYBRIDSLEEP, PowerMode.DC, 0);
                //SUB_SLEEP - HIBERNATEIDLE
                PowerManager.SetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.HIBERNATEIDLE, PowerMode.AC, 0);
                PowerManager.SetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.HIBERNATEIDLE, PowerMode.DC, 0);

                //SUB_VIDEO - VIDEOIDLE
                PowerManager.SetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.VIDEO_SUBGROUP, Setting.VIDEOIDLE, PowerMode.AC, 0);
                PowerManager.SetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.VIDEO_SUBGROUP, Setting.VIDEOIDLE, PowerMode.DC, 0);
            };

            Func<bool> actionValidation = 
            () => 
            {
                return
                //SUB_DISK - DISKIDLE
                PowerManager.GetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.DISK_SUBGROUP, Setting.DISKIDLE, PowerMode.AC) == 0
                &&
                PowerManager.GetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.DISK_SUBGROUP, Setting.DISKIDLE, PowerMode.DC) == 0
                &&
                //SUB_SLEEP - STANDBYIDLE
                PowerManager.GetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.STANDBYIDLE, PowerMode.AC) == 0
                &&
                PowerManager.GetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.STANDBYIDLE, PowerMode.DC) == 0
                &&
                //SUB_SLEEP - HYBRIDSLEEP
                PowerManager.GetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.HYBRIDSLEEP, PowerMode.AC) == 0
                &&
                PowerManager.GetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.HYBRIDSLEEP, PowerMode.DC) == 0
                &&
                //SUB_SLEEP - HIBERNATEIDLE
                PowerManager.GetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.HIBERNATEIDLE, PowerMode.AC) == 0
                &&
                PowerManager.GetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.SLEEP_SUBGROUP, Setting.HIBERNATEIDLE, PowerMode.DC) == 0
                &&
                //SUB_VIDEO - VIDEOIDLE
                PowerManager.GetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.VIDEO_SUBGROUP, Setting.VIDEOIDLE, PowerMode.AC) == 0
                &&
                PowerManager.GetPlanSetting(
                    PowerManager.GetActivePlan(),
                    SettingSubgroup.VIDEO_SUBGROUP, Setting.VIDEOIDLE, PowerMode.DC) == 0;
            };

            return new ComponentTask(new SetupTask(prequisiteFunc, action, actionValidation, false), null);
        }

        private ComponentTask ApplyContextMenuModifyingRegistryEdits()
        {
            Func<bool> prequisiteFunc = null;

            Action action =
            () =>
            {
                Registry.SetValue(@"HKEY_CLASSES_ROOT\AllFilesystemObjects\shellex\ContextMenuHandlers\Copy To", "", "{C2FBB630-2971-11D1-A18C-00C04FD75D13}", RegistryValueKind.String);
                Registry.SetValue(@"HKEY_CLASSES_ROOT\AllFilesystemObjects\shellex\ContextMenuHandlers\Move To", "", "{C2FBB631-2971-11D1-A18C-00C04FD75D13}", RegistryValueKind.String);
            };

            Func<bool> actionValidation =
            () =>
            {
                return
                (string)Registry.GetValue(@"HKEY_CLASSES_ROOT\AllFilesystemObjects\shellex\ContextMenuHandlers\Copy To", "", null) == "{C2FBB630-2971-11D1-A18C-00C04FD75D13}"
                &&
                (string)Registry.GetValue(@"HKEY_CLASSES_ROOT\AllFilesystemObjects\shellex\ContextMenuHandlers\Move To", "", null) == "{C2FBB631-2971-11D1-A18C-00C04FD75D13}";
            };

            return new ComponentTask(new SetupTask(prequisiteFunc, action, actionValidation, false), null);
        } 

        private ComponentTask ApplyPerformanceImprovingRegistryEdits()
        {
            Func<bool> prequisiteFunc = null;

            Action action =
            () =>
            {
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "AutoEndTasks", 1);
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "HungAppTimeout", 1000);
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "MenuShowDelay", 8);
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "WaitToKillAppTimeout", 2000);
                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "LowLevelHooksTimeout", 1000);

                Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseHoverTime", 8);

                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoLowDiskSpaceChecks", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "LinkResolveIgnoreLinkInfo", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoResolveSearch", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoResolveTrack", 1, RegistryValueKind.DWord);
                Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoInternetOpenWith", 1, RegistryValueKind.DWord);

                Registry.SetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control", "WaitToKillServiceTimeout", 2000);
            };

            Func<bool> actionValidation =
            () =>
            {
                return
                (int)Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "AutoEndTasks", null) == 1
                &&
                (int)Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "HungAppTimeout", null) == 1000
                &&
                (int)Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "MenuShowDelay", null) == 8
                &&
                (int)Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "WaitToKillAppTimeout", null) == 2000
                &&
                (int)Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "LowLevelHooksTimeout", null) == 1000
                &&
                (int)Registry.GetValue(@"HKEY_CURRENT_USER\Control Panel\Mouse", "MouseHoverTime", null) == 8
                &&
                (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoLowDiskSpaceChecks", null) == 1
                &&
                (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "LinkResolveIgnoreLinkInfo", null) == 1
                &&
                (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoResolveSearch", null) == 1
                &&
                (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoResolveTrack", null) == 1
                &&
                (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoInternetOpenWith", null) == 1
                &&
                (int)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control", "WaitToKillServiceTimeout", null) == 2000;
            };

            return new ComponentTask(new SetupTask(prequisiteFunc, action, actionValidation, false), null);
        }

        private ComponentTask CopyWindowsFiles()
        {
            Func<bool> prequisiteFunc = null;

            Action action =
            () =>
            {
                SystemUtil.CopyFolderContents(new DirectoryInfo(SetupProperties.Oscopy + "Windows"), new DirectoryInfo(SystemPathConstants.WindowsPath), false);
            };

            Func<bool> actionValidation =
            () =>
            {
                return 
                File.Exists(@"C:\Windows\sleep.exe")
                &&
                File.Exists(@"C:\Windows\WAR.exe")
                &&
                File.Exists(@"C:\Windows\CheckTinybotVersion.exe")
                &&
                File.Exists(@"C:\Windows\QRes.exe");
            };

            return new ComponentTask(new SetupTask(prequisiteFunc, action, actionValidation, false), null);
        }

        private ComponentTask ScheduleStartupTasks()
        {
            Func<bool> prequisiteFunc = null;

            Action action =
            () =>
            {
                Registry.SetValue(RegistryConstants.HKLM_RUN_PATH, "WAR", SystemPathConstants.CmdPathCommand + "\"C:\\Windows\\WAR.exe\"");
            };

            Func<bool> actionValidation =
            () =>
            {
                return
                (string)Registry.GetValue(RegistryConstants.HKLM_RUN_PATH, "WAR", null) == 
                    SystemPathConstants.CmdPathCommand + "\"C:\\Windows\\WAR.exe\"";
            };

            return new ComponentTask(new SetupTask(prequisiteFunc, action, actionValidation, false), null);
        }
        
        /*private CleanupTask Cleanup()
        {
            if (Directory.Exists(SetupProperties.Os))
            {
                Directory.Delete(SetupProperties.Os, true);
            }
        }*/

        public static bool VerifyOS(bool silent)
        {
            var sleepExists = File.Exists(Path.Combine(SystemPathConstants.WindowsPath, "sleep.exe"));
            var WARExists = File.Exists(Path.Combine(SystemPathConstants.WindowsPath, "WAR.exe"));
            var CheckTinybotVersionExists = File.Exists(Path.Combine(SystemPathConstants.WindowsPath, "CheckTinybotVersion.exe"));
            var QResExists = File.Exists(Path.Combine(SystemPathConstants.WindowsPath, "QRes.exe"));
            var CheckTinybotVersionShortcutExists = File.Exists(@"C:\Users\Public\Desktop\Check Tinybot Version.lnk");
            var key = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, RegistryConstants.HKLM_RUN_PATH);
            var WarRunKeyExists = RegistryUtil.RegistryKeyValueDataExists(key, "WAR", "C:\\Windows\\System32\\cmd.exe /c \"C:\\Windows\\WAR.exe\"");

            bool allExists = true;

            if (((sleepExists) == false))
            {
                if ((silent) == false)
                {
                    Console.Out.WriteLine("Sleep.exe is missing...");
                }
                allExists = false;
            }

            if (((WARExists) == false))
            {
                if ((silent) == false)
                {
                    Console.Out.WriteLine("WAR.exe is missing...");
                }
                allExists = false;
            }

            if (((CheckTinybotVersionExists) == false))
            {
                if ((silent) == false)
                {
                    Console.Out.WriteLine("CheckTinybotVersion.exe is missing...");
                }
                allExists = false;
            }

            if (((QResExists) == false))
            {
                if ((silent) == false)
                {
                    Console.Out.WriteLine("QRes.exe is missing...");
                }
                allExists = false;
            }

            if (((CheckTinybotVersionShortcutExists) == false))
            {
                if ((silent) == false)
                {
                    Console.Out.WriteLine("Check Tinybot Version.lnk is missing...");
                }
                allExists = false;
            }

            if (((WarRunKeyExists) == false))
            {
                if ((silent) == false)
                {
                    Console.Out.WriteLine("WAR.exe's Run registry key is missing...");
                }
                allExists = false;
            }

            if ((allExists) == true)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
