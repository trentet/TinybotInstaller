using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TinybotInstaller
{
    class Class1
    {
        static string PSScriptRoot = "PSScriptRoot"; //Split-Path -Parent -Path MyInvocation.MyCommand.Definition;
        static string setup = Path.Combine(PSScriptRoot + @"\Setup\");

        static string config = Path.Combine(setup, @"Config\");

        static string firefox = Path.Combine(config + @"Firefox\");
        static string firefoxinstall = @"C:\Program Files\Mozilla Firefox\";
        static string os = Path.Combine(config, @"OS\");
        static string oscopy = Path.Combine(os, @"Copy\");
        static string pins = Path.Combine(config, @"Pins\");

        static string install = Path.Combine(setup + @"Install\");

        static string rs = Path.Combine(install, @"RS\");
        static string rsinstall = @"C:\Users\Administrator\jagexcache\jagexlauncher\bin\JagexLauncher.exe";
        static string osrsprm = @"C:\Users\Administrator\jagexcache\jagexlauncher\oldschool\oldschool.prm";
        static string rs3prm = @"C:\Users\Administrator\jagexcache\jagexlauncher\runescape\runescape.prm";

        static string chocolatey = Path.Combine(install, @"Chocolatey\");
        static string chocolateyinstallscript = Path.Combine(chocolatey, "InstallChocolatey.ps1");
        static string chocolateylocaltemp = @"C:\Users\Administrator\AppData\Local\Temp\chocolatey";

        static string startup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\Microsoft\Windows\Start Menu\Programs\Startup\");
        static string windows = @"C:\Windows\";

        static string runKey = "Run";
        static string runOnceKey = "RunOnce";
        static string hklm_WindowsCurrentVersion = @"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion";
        static string hkcu_WindowsCurrentVersion = @"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion";
        static string hklm_Run = Path.Combine(hklm_WindowsCurrentVersion, runKey);
        static string hklm_RunOnce = Path.Combine(hklm_WindowsCurrentVersion, runOnceKey);
        static string hkcu_RunOnce = Path.Combine(hkcu_WindowsCurrentVersion, runOnceKey);
        static string hklm_WindowsNTCurrentVersion = @"HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion";

        static string tinybotVersionKey = "CurrentVersionTinybot";
        static List<string> compatibleTinybotUpdgradeVersions = new List<string> { "4.4 Base 1" };
        static string newTinybotVersion = "4.4 R1";

        static string x64JavaFolderPath = @"C:\Program Files\Java\jre*\bin";
        static string x86JavaFolderPath = @"C:\Program Files (x86)\Java\jre*\bin";

        static string cmdPath = @"C:\Windows\System32\cmd.exe";
        static string cmdPathCommand = (cmdPath + " /c ");

        static bool cancelsetup = false;

        static int setWidth = 1280;
        static int setHeight = 720;

        static string localChocolateyPackageFilePath = "";

        public partial class NativeMethods
        {
            /// Return Type: BOOL->int
            ///fBlockIt: BOOL->int
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "BlockInput")]
            [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool BlockInput([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)] bool fBlockIt);
        }

        public static void BlockInput(TimeSpan span)
        {
            try
            {
                NativeMethods.BlockInput(true);
                Thread.Sleep(span);
            }
            finally
            {
                NativeMethods.BlockInput(false);
            }
        }

        public void TinybotSetup()
        {
            var newTinybotVersion = new Version(5, 5, 0, 0);
            if (GetPendingReboot() == true)
            {
                Console.WriteLine("A reboot is pending. Will run TinybotInstaller at next logon...");
            }
            else
            {
                /*
                    Start-Transaction -RollbackPreference Error
                */
                RegistryKey key = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Services\Mouclass");
                key.SetValue("Start", 3, RegistryValueKind.DWord);
                DisableUserInput();
                Console.WriteLine("Beginning setup for TinybotW10 v" + newTinybotVersion.ToString());
                Console.WriteLine(@"Setup logs are located at: C:\Windows\Logs\TBSetup.log");
                RegistryKey winNTCurrentVersion = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, hklm_WindowsNTCurrentVersion);
                var currentTBVersion = String.Join("", (string[])winNTCurrentVersion.GetValue(Path.Combine(hklm_WindowsNTCurrentVersion, tinybotVersionKey)));

                if (compatibleTinybotUpdgradeVersions.Contains(currentTBVersion) == true)
                {
                    Console.WriteLine(@"Initializing setup. User input is disabled during this process. DO NOT SHUTDOWN OR RESTART! Script will automatically restart upon succesful completion. If setup hangs at any step longer than 30 minutes, please notify Trent! from RiD. Setup logs are located at: C:\Windows\Logs\TBSetup.log");
                    //Async-Open-MessageBox -Message ("Initializing setup. User input is disabled during this process. DO NOT SHUTDOWN OR RESTART! Script will automatically restart upon succesful completion. If setup hangs at any step longer than 30 minutes, please notify Trent! from RiD. Setup logs are located at: C:\Windows\Logs\TBSetup.log")
                    Console.WriteLine("===============================================================================");
                    SetupOS();
                    Console.WriteLine("===============================================================================");
                    SetupRS();
                    Console.WriteLine("===============================================================================");
                    Console.WriteLine("Verifying network connectivity before installing Chocolatey");
                    if (Network.TestInternetConnection() == true)
                    {
                        Console.WriteLine("Network connectivity confirmed...");
                        SetupChocolatey(localChocolateyPackageFilePath);
                        Console.WriteLine("===============================================================================");
                        Console.WriteLine("Checking for pre-existing Mozilla Firefox install...");
                        if (InstallerUtil.IsSoftwareInstalled(InstallerUtil.Architectures.X64, "Mozilla Firefox * (x64 *)") == true)
                        {
                            Console.WriteLine("64-bit Mozilla Firefox is already installed. Skipping...");
                        }
                        else
                        {
                            Console.WriteLine("64-bit Mozilla Firefox not found. Attempting to install...");
                            var firefoxInstalled = ChocoUpgrade("firefox", "Mozilla Firefox", true, "Mozilla Firefox * (x64 *)", true);
                            if ((firefoxInstalled) == false)
                            {
                                Console.WriteLine("Failed to install Mozilla Firefox. Please install manually or open CMD and type: choco upgrade firefox --force --y --ignorechecksum");
                                UndoTransaction();
                                CancelSetup();
                            }
                            else
                            {
                                Console.WriteLine("Mozilla Firefox is installed.");
                                ConfigureFirefox();
                            }
                        }
                        Console.WriteLine("===============================================================================");
                        Console.WriteLine("Checking for pre-existing WinRAR install...");
                        if (InstallerUtil.IsSoftwareInstalled(InstallerUtil.Architectures.X64, "WinRAR*") == true)
                        {
                            Console.WriteLine("64-bit WinRAR is already installed. Skipping...");
                        }
                        else
                        {
                            Console.WriteLine("64-bit WinRAR not found. Attempting to install...");
                            var winrarInstalled = ChocoUpgrade("winrar", "WinRAR", true, "WinRAR*", true);
                            if ((winrarInstalled) == false)
                            {
                                Console.WriteLine("Failed to install WinRAR. Please install manually or open CMD and type: choco upgrade winrar --force --y --ignorechecksum");
                                UndoTransaction();
                                CancelSetup();
                            }
                            else
                            {
                                Console.WriteLine("WinRAR is installed.");
                            }
                        }
                        Console.WriteLine("===============================================================================");
                        Console.WriteLine("Checking for pre-existing TeamViewer install...");
                        if (InstallerUtil.IsSoftwareInstalled(InstallerUtil.Architectures.X86, "TeamViewer*") == true)
                        {
                            Console.WriteLine("32-bit TeamViewer is already installed. Skipping...");
                        }
                        else
                        {
                            var teamviewerInstalled = ChocoUpgrade("teamviewer", "TeamViewer", true, "TeamViewer*", true);
                            if ((teamviewerInstalled) == false)
                            {
                                Console.WriteLine("Failed to install TeamViewer. Please install manually or open CMD and type: choco upgrade teamviewer --force --y --ignorechecksum");
                                UndoTransaction();
                                CancelSetup();
                            }
                            else
                            {
                                Console.WriteLine("TeamViewer is installed.");
                            }
                        }
                        Console.WriteLine("===============================================================================");
                        Console.WriteLine("Checking for pre-existing 32-bit Java SE 8 install...");

                        if (!JavaInstallUtil.IsJavaInstalled(InstallerUtil.Architectures.X86, 8))
                        {
                            Console.WriteLine("32-bit Java is not installed. Attempting install...");
                            ChocoUpgrade("jre8", "Java SE 8", false, "Java 8 Update *", true);

                            Console.WriteLine("Verifying install...");
                            if (!JavaInstallUtil.IsJavaInstalled(InstallerUtil.Architectures.X86, 8))
                            {
                                Console.WriteLine("32-bit Java failed to install...");
                                UndoTransaction();
                                CancelSetup();
                            }
                            else
                            {
                                Console.WriteLine("32-bit Java installed successfully.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("32-bit Java is already installed.");
                        }
                        Console.WriteLine("Checking for pre-existing 64-bit Java SE 8 install...");
                        if (JavaInstallUtil.IsJavaInstalled(InstallerUtil.Architectures.BOTH, 8))
                        {
                            Console.WriteLine("64-bit Java is not installed. Attempting install...");
                            ChocoUpgrade("jre8", "Java SE 8", false, "Java 8 Update * (64-bit)", true);

                            Console.WriteLine("Verifying install...");
                            if (JavaInstallUtil.IsJavaInstalled(InstallerUtil.Architectures.BOTH, 8))
                            {
                                Console.WriteLine("64-bit Java failed to install...");
                                UndoTransaction();
                                CancelSetup();
                            }
                            else
                            {
                                Console.WriteLine("64-bit Java installed successfully.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("64-bit Java is already installed.");
                        }
                        Console.WriteLine("===============================================================================");
                        Console.WriteLine("Checking for pre-existing VMWare Tools install...");
                        if (InstallerUtil.IsSoftwareInstalled(InstallerUtil.Architectures.X64, "VMWare Tools") == true) { 
                            Console.WriteLine("64-bit VMWare Tools is already installed. Skipping...");
                        }
                        else
                        {
                            bool vmwareToolsInstalled = ChocoUpgrade("vmware-tools", "VMWare Tools", true, "VMWare Tools", false);
                            if ((vmwareToolsInstalled) == false)
                            {
                                Console.WriteLine("Failed to install VMWare Tools. Please install manually or open CMD and type: choco upgrade vmware-tools --force --y --ignorechecksum");
                                UndoTransaction();
                                CancelSetup();
                            }
                            else
                            {
                                Console.WriteLine("VMWare Tools is installed.");
                            }
                        }
                        Console.WriteLine("===============================================================================");
                        if (File.Exists(Path.Combine(pins, "Pins.vbs")))
                        {
                            SetupPins();
                        }
                        else
                        {
                            Console.WriteLine("Pinning setup files are either missing or already executed...");
                        }
                        Console.WriteLine("===============================================================================");
                        ScheduleCleanup();
                        Console.WriteLine("===============================================================================");
                        currentTBVersion = string.Join("", (string[])winNTCurrentVersion.GetValue(Path.Combine(hklm_WindowsNTCurrentVersion, tinybotVersionKey)));
                        if (compatibleTinybotUpdgradeVersions.Contains(currentTBVersion) == true)
                        {
                            Console.WriteLine("Setup complete. Updating Tinybot version to 4.4 R1...");
                            var tbVersionKey = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\");
                            tbVersionKey.SetValue(tinybotVersionKey, newTinybotVersion);

                            if (tbVersionKey.GetValue(tinybotVersionKey).Equals(newTinybotVersion))
                            {
                                Console.WriteLine("Tinybot version updated successfully!");
                                Console.WriteLine("Rebooting to apply changes...");
                                CompleteTransaction();
                                EnableUserInput();
                                RestartComputer(0);
                            }
                            else
                            {
                                Console.WriteLine("Failed to update Tinybot version...");
                                UndoTransaction();
                                CancelSetup();
                            }
                        }
                        else
                        {
                            Console.WriteLine("Incompatible Tinybot version... Cannot perform setup... Please download the newest version from the 'How to Minimize Your Bot' thread.");
                            UndoTransaction();
                            CancelSetup();
                        }
                    }
                    else
                    {
                        //Async-Open-MessageBox -Message ("Failed to confirm network connectivity. If your host has connection, try: VMWare Toolbar -> Edit -> Virtual Network Editor -> Change Settings -> Restore Defaults. Then Restart")
                        Console.WriteLine("Failed to confirm network connectivity. If your host has connection, try: VMWare Toolbar -> Edit -> Virtual Network Editor -> Change Settings -> Restore Defaults. Then Restart");
                        UndoTransaction();
                        CancelSetup();
                    }
                }
                else
                {
                    Console.WriteLine("Incompatible Tinybot version... Cannot perform setup... Please download the newest version from the 'How to Minimize Your Bot' thread.");
                    //AsyncOpenMessageBoxMessage("Incompatible Tinybot version... Cannot perform setup... Please download the newest version from the 'How to Minimize Your Bot' thread.")
                    Console.WriteLine("Incompatible Tinybot version... Cannot perform setup... Please download the newest version from the 'How to Minimize Your Bot' thread.");
                    UndoTransaction();
                    Cancelsetup();
                }
            }
        }
            
        public void RestartComputer(int delay)
        {
            System.Diagnostics.Process.Start("shutdown.exe", "-r -t " + delay);
        }

        public void CompleteTransaction()
        {

        }

        public string ExecuteCMDCommand(string command)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine("echo Oscar");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();

            return cmd.StandardOutput.ReadToEnd();
        }

        public static void CopyFolderContents(DirectoryInfo source, DirectoryInfo target, bool createDirectory)
        {
            if (createDirectory)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyFolderContents(diSourceSubDir, nextTargetSubDir, true);
            }
        }

        public void SetupOS()
        {
            try
            {
                if (VerifyOS(true) == false) {

                    Console.Out.WriteLine("Optimizing power options...");
                    ExecuteCMDCommand("powercfg.exe -change -monitor-timeout-ac 0");
                    ExecuteCMDCommand("powercfg.exe -change -standby-timeout-ac 0");
                    ExecuteCMDCommand("powercfg.exe -change -disk-timeout-ac 0");
                    ExecuteCMDCommand("powercfg.exe -change -hibernate-timeout-ac 0");

                    Console.Out.WriteLine("===============================================================================");

                    Console.Out.WriteLine("Applying registry edits...");

                    Registry.SetValue(@"HKEY_CLASSES_ROOT\AllFilesystemObjects\shellex\ContextMenuHandlers\Copy To", "", "{C2FBB630-2971-11D1-A18C-00C04FD75D13}", RegistryValueKind.String);

                    Registry.SetValue(@"HKEY_CLASSES_ROOT\AllFilesystemObjects\shellex\ContextMenuHandlers\Move To", "", "{C2FBB631-2971-11D1-A18C-00C04FD75D13}", RegistryValueKind.String);

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

                    Console.Out.WriteLine("===============================================================================");

                    Console.Out.WriteLine(@"Copying files to C:\Windows\...");

                    CopyFolderContents(new DirectoryInfo(oscopy + "Windows"), new DirectoryInfo(windows), false);
                    /*CopyItemPath($oscopy + "Windows\sleep.exe")Destination $windows Force
                    CopyItemPath($oscopy + "Windows\WAR.exe")Destination $windows Force
                    CopyItemPath($oscopy + "Windows\CheckTinybotVersion.exe")Destination $windows Force
                    CopyItemPath($oscopy + "Windows\QRes.exe")Destination $windows Force*/

                    Console.Out.WriteLine("===============================================================================");
                    Console.Out.WriteLine("Copying shortcuts to Desktop...");
                    CopyFolderContents(new DirectoryInfo(Path.Combine(oscopy + @"Public Desktop")), new DirectoryInfo(@"C:\Users\Public\Desktop"), false);
                    //CopyItemPath(oscopy + "Public Desktop\Check Tinybot Version.lnk")Destination "C:\Users\Public\Desktop\"
                    Console.Out.WriteLine("===============================================================================");
                    Console.Out.WriteLine("Scheduling startup tasks...");
                    Registry.SetValue(hklm_Run, "WAR", cmdPathCommand + "\"C:\\Windows\\WAR.exe\"");
                    if (VerifyOS(false) == true) {
                        if (Directory.Exists(os))
                        {
                            Directory.Delete(os, true);
                        }
                        CompleteTransaction();
                    }
                    else
                    {
                        //Console.Out.WriteLine("[0] OS configuration failed... Rolling back...");
                        //Console.Out.WriteLine($_.Exception.GetType().FullName, $_.Exception.Message);
                        UndoTransaction();
                        CancelSetup();
                        throw new Exception("[0] OS configuration failed... Rolling back...");
                    }
                }
                else
                {
                   Console.Out.WriteLine("OS Configuration has already been completed. Skipping...");
                   CompleteTransaction();
              }
            }
            catch
            {
                //Console.Out.WriteLine("[1] OS configuration failed... Rolling back...");
                //Console.Out.WriteLine($_.Exception.GetType().FullName, $_.Exception.Message);
                UndoTransaction();
                CancelSetup();
                throw new Exception("[1] OS configuration failed... Rolling back...");
            }
        }

        public void CancelSetup()
        {

        }

        public void UndoTransaction()
        {

        }

        public bool GetPendingReboot()
        {
            /*
             Check "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\" if a subkey named "RebootPending" is present. 
             Check "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\" if a subkey named "RebootRequired" is present. 
             Check "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\" if a value "PendingFileRenameOperations" if present. 
             If any of these conditions are met, a reboot is required.
            */

            bool hasPendingReboot = false;

            RegistryKey cbsKey = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing");
            RegistryKey wuauKey = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update");
            RegistryKey smssKey = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\Session Manager");

            if (cbsKey.OpenSubKey("RebootPending") != null || wuauKey.OpenSubKey("RebootRequired") != null || smssKey.GetValue("PendingFileRenameOperations") != null)
            {
                hasPendingReboot = true;
            }

            return hasPendingReboot;
        }

        public bool VerifyOS(bool silent)
        {
            var sleepExists = File.Exists(windows + "sleep.exe");
            var WARExists = File.Exists(windows + "WAR.exe");
            var CheckTinybotVersionExists = File.Exists(windows + "CheckTinybotVersion.exe");
            var QResExists = File.Exists(windows + "QRes.exe");
            var CheckTinybotVersionShortcutExists = File.Exists(@"C:\Users\Public\Desktop\Check Tinybot Version.lnk");
            var key = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, hklm_Run);
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

        public void AsyncInstallerTimeout()
        {

        }

        public void SetupRS()
        {
            //Start-Transaction -RollbackPreference Error
            try
            {
                if (VerifyOSRS() == false)
                {
                    Console.Out.WriteLine("Installing OSRS client...");
                    //Start - Job - ScriptBlock scriptBlock - ArgumentList @(90, "Old School RuneScape", "InstallOSRS") | Out - Null
                    ProcessStartInfo start_info = new ProcessStartInfo(Path.Combine(rs, "InstallOSRS.exe"));
                    Process proc = new Process();
                    proc.StartInfo = start_info;
                    proc.Start();
                    proc.WaitForExit();

                    foreach (var process in Process.GetProcessesByName("iexplore"))
                    {
                        process.Kill();
                    }
                }
                else
                {
                    Console.Out.WriteLine("Old School RuneScape is already installed... Skipping...");
                }

                if (VerifyRS3() == false)
                {
                    Console.Out.WriteLine("Installing RS3 client...");
                    //Start - Job - ScriptBlock scriptBlock - ArgumentList @(90, "RuneScape 3", "InstallRS3") | Out - Null
                    ProcessStartInfo start_info = new ProcessStartInfo(Path.Combine(rs, "InstallRS3.exe"));
                    Process proc = new Process();
                    proc.StartInfo = start_info;
                    proc.Start();
                    proc.WaitForExit();

                    foreach (var process in Process.GetProcessesByName("iexplore"))
                    {
                        process.Kill();
                    }
                }
                else
                {
                    Console.Out.WriteLine("RuneScape 3 is already installed... Skipping...");
                }

                if (VerifyOSRS() == true && VerifyRS3() == true)
                {
                    Console.Out.WriteLine("Installation of Old School RS and RuneScape 3 was successful...");
                    if (Directory.Exists(rs))
                    {
                        Directory.Delete(rs, true);
                    }
                    CompleteTransaction();
                }
                else
                {
                    if ((VerifyOSRS()) == false)
                    {
                        throw new Exception("[0] OSRS installation failed... Rolling back...");
                    }
                    if ((VerifyRS3()) == false)
                    {
                        throw new Exception("[0] RS3 installation failed... Rolling back...");
                    }

                    //Console.Out.WriteLine(_.Exception.GetType().FullName, _.Exception.Message
                    UndoTransaction();
                    CancelSetup();
                }
            }
            catch
            {
                //Console.Out.WriteLine(_.Exception.GetType().FullName, _.Exception.Message
                UndoTransaction();
                CancelSetup();
                throw new Exception("[1] RS installation failed... Rolling back...");
            }
        }

        public bool VerifyOSRS()
        {
            if (File.Exists(rsinstall) && File.Exists(osrsprm))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool VerifyRS3()
        {
            if (File.Exists(rsinstall) && File.Exists(rs3prm))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetupChocolatey(string localChocolateyPackageFilePath)
        {

        }

        public bool ChocoUpgrade(string pkgName, string displayName, bool verifyInstall, string searchName, bool allowWildcards)
        {

            return true;
        }

        public void ConfigureFirefox()
        {
            //Start-Transaction -RollbackPreference Error
            try
            {
                if (VerifyFirefox() == false)
                {
                    Console.Out.WriteLine("Copying Mozilla Firefox configuration files...");
                    File.Copy(Path.Combine(firefox, @"browser\override.ini"), Path.Combine(firefoxinstall, @"browser\override.ini"), true);
                    File.Copy(Path.Combine(firefox, @"defaults\pref\autoconfig.js"), Path.Combine(firefoxinstall, @"defaults\pref\autoconfig.js"), true);
                    File.Copy(Path.Combine(firefox, @"firefox.cfg"), firefoxinstall, true);

                    Console.Out.WriteLine("Setting Mozilla Firefox as default browser...");
                    ProcessStartInfo start_info = new ProcessStartInfo(Path.Combine(firefoxinstall, @"uninstall\helper.exe"));
                    start_info.Arguments = "/SetAsDefaultAppGlobal";
                    Process proc = new Process();
                    proc.StartInfo = start_info;
                    proc.Start();
                    proc.WaitForExit();

                    start_info.Arguments = "/SetAsDefaultAppUser";
                    proc.StartInfo = start_info;
                    proc.Start();
                    proc.WaitForExit();
        
                    if (VerifyFirefox() == true)
                    {
                        Console.Out.WriteLine("Custom configurations successfully applied to Mozilla Firefox.");
                        if (Directory.Exists(firefox))
                        {
                            Directory.Delete(firefox);
                        }
                        CompleteTransaction();
                    }
                    else
                    {
                        //Console.Out.WriteLine(_.Exception.GetType().FullName, _.Exception.Message
                        UndoTransaction();
                        CancelSetup();
                        throw new Exception("[0] Firefox configuration failed... Rolling back...");
                    }
                }
                else
                {
                    Console.Out.WriteLine("Firefox is already configured. Skipping...");
                }
            }
            catch
            {
                //Console.Out.WriteLine(_.Exception.GetType().FullName, _.Exception.Message
                UndoTransaction();
                CancelSetup();
                throw new Exception("[1] Firefox configuration failed... Rolling back...");
            }
        }

        public bool VerifyFirefox()
        {
            bool isVerified = false;
            if (File.Exists(Path.Combine(firefoxinstall, @"browser\override.ini")) && 
                File.Exists(Path.Combine(firefoxinstall, @"defaults\pref\autoconfig.js")) && 
                File.Exists(Path.Combine(firefoxinstall, @"firefox.cfg")))
            {
                return isVerified;
            }

            return isVerified;
        }

        public void SetupPins()
        {
            //Start-Transaction -RollbackPreference Error
            try
            {
                Console.Out.WriteLine("Pinning apps to taskbar...");
                if (File.Exists(@"C:\Users\Public\Desktop\Firefox.lnk") && 
                    File.Exists(@"C:\Users\Administrator\Desktop\OldSchool RuneScape.lnk") && 
                    File.Exists(@"C:\Users\Administrator\Desktop\RuneScape.lnk"))
                {
                    if (File.Exists(pins))
                    {
                        //cscript(pins + "Pins.vbs")
                        Thread.Sleep(3000);
                        if (File.Exists(pins))
                        {
                            Directory.Delete(pins, true);
                        }
                    }
                    else
                    {
                        Console.Out.WriteLine("Files required for pinning are missing. This could occur if pins have already been applied. Please notify Trent! if Firefox, OSRS, and RS3 were were not pinned to taskbar.");
                    }
                    CompleteTransaction();
                }
                else
                {
                    if (!(File.Exists(@"C:\Users\Public\Desktop\Firefox.lnk")))
                    {
                        Console.Out.WriteLine(@"Firefox shortcut missing. Pins were not applied.");
                    }
                    if (!(File.Exists(@"C:\Users\Administrator\Desktop\OldSchool RuneScape.lnk")))
                    {
                        Console.Out.WriteLine("OldSchool RuneScape shortcut missing. Pins were not applied.");
                    }
                    if (!(File.Exists(@"C:\Users\Administrator\Desktop\RuneScape.lnk")))
                    {
                        Console.Out.WriteLine("RuneScape 3 shortcut missing. Pins were not applied.");
                    }
                    //Console.Out.WriteLine(_.Exception.GetType().FullName, _.Exception.Message
                    UndoTransaction();
                    CancelSetup();
                    throw new Exception("[0] Applying pins failed... Rolling back...");
                }
            }
            catch
            {
                //Console.Out.WriteLine(_.Exception.GetType().FullName, _.Exception.Message
                UndoTransaction();
                CancelSetup();
                throw new Exception("[1] Applying pins failed... Rolling back...");
            }
        }

        public void ScheduleCleanup()
        {
            //Start-Transaction -RollbackPreference Error
            try
            {
                Console.Out.WriteLine("Scheduling cleanup...");
                if (File.Exists(setup))
                {
                    Directory.Delete(setup, true);
                }

                Registry.SetValue(hklm_RunOnce, "CleanupFirstSetup", cmdPathCommand + "del /q \"" + startup + "FirstSetup.exe\"", RegistryValueKind.String);
                Registry.SetValue(hklm_Run, "CleanupChocolatey", cmdPathCommand + "rmdir /s /q \"" + chocolateylocaltemp + "\"", RegistryValueKind.String);
                Registry.SetValue(hklm_RunOnce, "CleanupScriptFolder", cmdPathCommand + "rmdir /s /q \"" + PSScriptRoot + "\"", RegistryValueKind.String);
                Registry.SetValue(hklm_RunOnce, "CleanupLocalAppDataTemp1", cmdPathCommand + "del /S /Q \"C:\\Users\\Administrator\\AppData\\Local\\Temp\\*\"", RegistryValueKind.String);
                Registry.SetValue(hklm_RunOnce, "CleanupWindowsTemp1", cmdPathCommand + "del /S /Q \"C:\\Windows\\Temp\\*\"", RegistryValueKind.String);
                Registry.SetValue(hklm_RunOnce, "CleanupLocalAppDataTemp2", cmdPathCommand + "for /d %x in (\"C:\\Users\\Administrator\\AppData\\Local\\Temp\\*\") do @rd /s /q \"%x\"", RegistryValueKind.String);
                Registry.SetValue(hklm_RunOnce, "CleanupWindowsTemp2", cmdPathCommand + "for /d %x in (\"C:\\Windows\\Temp\\*\") do @rd /s /q \"%x\"");
                Registry.SetValue(hkcu_RunOnce, "SetResolution", cmdPathCommand + "sleep 3 && QRes.exe /x:" + setWidth + " /y:" + setHeight + " && taskkill /f /IM explorer.exe && start explorer.exe", RegistryValueKind.String);
                CompleteTransaction();
            }
            catch
            {
                //Console.Out.WriteLine(_.Exception.GetType().FullName, _.Exception.Message
                UndoTransaction();
                CancelSetup();
                throw new Exception("[0] Cleanup scheduling failed... Rolling back...");
            }
        }

        public void DisableUserInput()
        {
            BlockInput(new TimeSpan());
        }

        public void EnableUserInput()
        {

        }

        public void Cancelsetup()
        {

        }
    }
}
