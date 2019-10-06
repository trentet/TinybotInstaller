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
        static bool cancelsetup = false;

        static int setWidth = 1280;
        static int setHeight = 720;

        //private Components.Firefox firefoxComponent = new Components.Firefox();
        //private Components.WinRAR winrarComponent = new Components.WinRAR();
        //private Components.TeamViewer teamViewerComponent = new Components.TeamViewer();
        //private Components.JavaX86 javaX86Component = new Components.JavaX86();
        //private Components.JavaX64 javaX64Component = new Components.JavaX64();

        public partial class NativeMethods
        {
            /// Return Type: BOOL->int
            ///fBlockIt: BOOL->int
            [System.Runtime.InteropServices.DllImportAttribute("user32.dll", EntryPoint = "BlockInput")]
            [return: System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)]
            public static extern bool BlockInput([System.Runtime.InteropServices.MarshalAsAttribute(System.Runtime.InteropServices.UnmanagedType.Bool)] bool fBlockIt);
        }

        public void TinybotSetup()
        {
            var newTinybotVersion = new Version(5, 5, 0, 0);
            if (SystemUtil.GetPendingReboot() == true)
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
                RegistryKey winNTCurrentVersion = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, RegistryConstants.TINYBOT_VERSION_KEY_PATH);
                string currentTBVersion = string.Join("", (string[])winNTCurrentVersion.GetValue(RegistryConstants.TINYBOT_VERSION_KEY));

                if (SetupProperties.CompatibleTinybotUpdgradeVersions.Contains(currentTBVersion) == true)
                {
                    Console.WriteLine(@"Initializing setup. User input is disabled during this process. DO NOT SHUTDOWN OR RESTART! Script will automatically restart upon succesful completion. If setup hangs at any step longer than 30 minutes, please notify Trent! from RiD. Setup logs are located at: C:\Windows\Logs\TBSetup.log");
                    //Async-Open-MessageBox -Message ("Initializing setup. User input is disabled during this process. DO NOT SHUTDOWN OR RESTART! Script will automatically restart upon succesful completion. If setup hangs at any step longer than 30 minutes, please notify Trent! from RiD. Setup logs are located at: C:\Windows\Logs\TBSetup.log")
                    Console.WriteLine("===============================================================================");
                    //os.Setup();
                    Console.WriteLine("===============================================================================");
                    //osrs.Setup();
                    Console.WriteLine("===============================================================================");
                    //rs3.Setup();
                    Console.WriteLine("===============================================================================");
                    Console.WriteLine("Verifying network connectivity before installing Chocolatey");
                    if (Network.TestInternetConnection() == true)
                    {
                        Console.WriteLine("Network connectivity confirmed...");
                        ChocoUtil.SetupChocolatey(ChocoUtil.LocalChocolateyPackageFilePath);
                        Console.WriteLine("===============================================================================");
                        //firefoxComponent.Setup();
                        Console.WriteLine("===============================================================================");
                        //winrarComponent.Setup();
                        Console.WriteLine("===============================================================================");
                        //teamViewerComponent.Setup();
                        Console.WriteLine("===============================================================================");
                        //javaX86Component.ComponentTasks.();
                        Console.WriteLine("===============================================================================");
                        //javaX64Component.Setup();
                        Console.WriteLine("===============================================================================");
                        //vmwware tools component here
                        Console.WriteLine("===============================================================================");
                        ScheduleCleanup();
                        Console.WriteLine("===============================================================================");
                        currentTBVersion = string.Join("", (string[])winNTCurrentVersion.GetValue(Path.Combine(RegistryConstants.HKLM_WINDOWSNTCURRENTVERSION_PATH, RegistryConstants.TINYBOT_VERSION_KEY)));
                        if (SetupProperties.CompatibleTinybotUpdgradeVersions.Contains(currentTBVersion) == true)
                        {
                            Console.WriteLine("Setup complete. Updating Tinybot version to 4.4 R1...");
                            var tbVersionKey = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\");
                            tbVersionKey.SetValue(RegistryConstants.TINYBOT_VERSION_KEY, newTinybotVersion);

                            if (tbVersionKey.GetValue(RegistryConstants.TINYBOT_VERSION_KEY).Equals(newTinybotVersion))
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
                    CancelSetup();
                }
            }
        }
            
        public static void RestartComputer(int delay)
        {
            System.Diagnostics.Process.Start("shutdown.exe", "-r -t " + delay);
        }

        public static void CompleteTransaction()
        {

        }

        public static void UndoTransaction()
        {

        }

        public static void AsyncInstallerTimeout()
        {

        }

        public static void CancelSetup()
        {
            cancelsetup = true;
        }
        
        public static void SetupPins()
        {
            //Start-Transaction -RollbackPreference Error
            try
            {
                Console.Out.WriteLine("Pinning apps to taskbar...");

                if (File.Exists(@"C:\Users\Public\Desktop\Firefox.lnk") && 
                    File.Exists(@"C:\Users\Administrator\Desktop\OldSchool RuneScape.lnk") && 
                    File.Exists(@"C:\Users\Administrator\Desktop\RuneScape.lnk"))
                {
                    if (File.Exists(SetupProperties.Pins))
                    {
                        //cscript(pins + "Pins.vbs")
                        Thread.Sleep(3000);
                        if (File.Exists(SetupProperties.Pins))
                        {
                            Directory.Delete(SetupProperties.Pins, true);
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

        public static void ScheduleCleanup()
        {
            //Start-Transaction -RollbackPreference Error
            try
            {
                Console.Out.WriteLine("Scheduling cleanup...");
                if (File.Exists(SetupProperties.Setup))
                {
                    Directory.Delete(SetupProperties.Setup, true);
                }

                Registry.SetValue(RegistryConstants.HKLM_RUN_ONCE_PATH, "CleanupFirstSetup", SystemPathConstants.CmdPathCommand + "del /q \"" + SystemPathConstants.StartupPath + "FirstSetup.exe\"", RegistryValueKind.String);
                Registry.SetValue(RegistryConstants.HKLM_RUN_PATH, "CleanupChocolatey", SystemPathConstants.CmdPathCommand + "rmdir /s /q \"" + ChocoUtil.ChocolateyLocalTemp + "\"", RegistryValueKind.String);
                Registry.SetValue(RegistryConstants.HKLM_RUN_ONCE_PATH, "CleanupScriptFolder", SystemPathConstants.CmdPathCommand + "rmdir /s /q \"" + SetupProperties.ExecutionPath + "\"", RegistryValueKind.String);
                Registry.SetValue(RegistryConstants.HKLM_RUN_ONCE_PATH, "CleanupLocalAppDataTemp1", SystemPathConstants.CmdPathCommand + "del /S /Q \"C:\\Users\\Administrator\\AppData\\Local\\Temp\\*\"", RegistryValueKind.String);
                Registry.SetValue(RegistryConstants.HKLM_RUN_ONCE_PATH, "CleanupWindowsTemp1", SystemPathConstants.CmdPathCommand + "del /S /Q \"C:\\Windows\\Temp\\*\"", RegistryValueKind.String);
                Registry.SetValue(RegistryConstants.HKLM_RUN_ONCE_PATH, "CleanupLocalAppDataTemp2", SystemPathConstants.CmdPathCommand + "for /d %x in (\"C:\\Users\\Administrator\\AppData\\Local\\Temp\\*\") do @rd /s /q \"%x\"", RegistryValueKind.String);
                Registry.SetValue(RegistryConstants.HKLM_RUN_ONCE_PATH, "CleanupWindowsTemp2", SystemPathConstants.CmdPathCommand + "for /d %x in (\"C:\\Windows\\Temp\\*\") do @rd /s /q \"%x\"");
                Registry.SetValue(RegistryConstants.HKCU_RUN_ONCE_PATH, "SetResolution", SystemPathConstants.CmdPathCommand + "sleep 3 && QRes.exe /x:" + setWidth + " /y:" + setHeight + " && taskkill /f /IM explorer.exe && start explorer.exe", RegistryValueKind.String);
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

        public static void DisableUserInput(TimeSpan span, bool bIsSilent = false)
        {
            try
            {
               if (!bIsSilent)
                {
                    Console.Out.WriteLine("Disabling mouse and keyboard for " + span.ToString());
                }
                DisableUserInput();
                Thread.Sleep(span);
            }
            finally
            {
                EnableUserInput();
            }
        }

        public static void DisableUserInput()
        {
            try
            {
                NativeMethods.BlockInput(true);
            }
            catch(Exception ex)
            {
                NativeMethods.BlockInput(false);
                throw ex;
            }
        }

        public static void EnableUserInput()
        {
            Console.Out.WriteLine("Enabling mouse and keyboard...");
            NativeMethods.BlockInput(false);
        }
    }
}
