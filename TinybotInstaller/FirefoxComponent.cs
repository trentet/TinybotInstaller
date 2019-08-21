using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinybotInstaller
{
    class FirefoxComponent
    {
        public static Class1.ComponentStatus Status { get; set; }

        public static void Setup()
        {
            Status = Class1.ComponentStatus.IN_PROGRESS;
            if (IsSetupSuccessful())
            {
                Console.WriteLine("All Firefox Component Setup tasks are already complete. Skipping...");
                Status = Class1.ComponentStatus.SKIPPED;
            }
            else
            {
                InstallFirefox();
                ConfigureFirefox();
                if (IsSetupSuccessful())
                {
                    Status = Class1.ComponentStatus.PASSED;
                }
                else
                {
                    Status = Class1.ComponentStatus.FAILED;
                }
            }
        }

        private static void InstallFirefox()
        {
            try
            {
                Console.WriteLine("Checking for pre-existing 64-bit Mozilla Firefox install...");
                if (IsFirefoxInstalled())
                {
                    Console.WriteLine("64-bit Mozilla Firefox is already installed. Skipping install...");
                }
                else
                {
                    Console.WriteLine("64-bit Mozilla Firefox not found. Attempting to install...");
                    ChocoUtil.ChocoUpgrade("firefox", "Mozilla Firefox", true, "Mozilla Firefox * (x64 *)", ChocoUtil.DefaultArgs);
                    if (IsFirefoxInstalled())
                    {
                        Console.WriteLine("64-bit Mozilla Firefox installed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("64-bit Mozilla Firefox failed to install. Please install manually or open CMD and type: choco upgrade firefox --force --y --ignorechecksum");
                    }
                }
            }
            catch
            {
                throw new Exception("[1] A critical error occurred while installing 64-bit Mozilla Firefox... Rolling back...");
            }
        }

        private static void ConfigureFirefox()
        {
            //Start-Transaction -RollbackPreference Error
            try
            {
                if (IsFirefoxInstalled())
                {
                    if (IsFirefoxConfigured() == false)
                    {
                        Console.Out.WriteLine("Copying Mozilla Firefox configuration files...");
                        File.Copy(Path.Combine(SetupProperties.Firefox, @"browser\override.ini"), Path.Combine(SetupProperties.Firefoxinstall, @"browser\override.ini"), true);
                        File.Copy(Path.Combine(SetupProperties.Firefox, @"defaults\pref\autoconfig.js"), Path.Combine(SetupProperties.Firefoxinstall, @"defaults\pref\autoconfig.js"), true);
                        File.Copy(Path.Combine(SetupProperties.Firefox, @"firefox.cfg"), SetupProperties.Firefoxinstall, true);

                        Console.Out.WriteLine("Setting Mozilla Firefox as default browser...");
                        ProcessStartInfo start_info = new ProcessStartInfo(Path.Combine(SetupProperties.Firefoxinstall, @"uninstall\helper.exe"));
                        start_info.Arguments = "/SetAsDefaultAppGlobal";
                        Process proc = new Process();
                        proc.StartInfo = start_info;
                        proc.Start();
                        proc.WaitForExit();

                        start_info.Arguments = "/SetAsDefaultAppUser";
                        proc.StartInfo = start_info;
                        proc.Start();
                        proc.WaitForExit();

                        if (IsFirefoxConfigured() == true)
                        {
                            Console.Out.WriteLine("Custom configurations successfully applied to Mozilla Firefox.");
                            if (Directory.Exists(SetupProperties.Firefox))
                            {
                                Directory.Delete(SetupProperties.Firefox);
                            }
                        }
                        else
                        {
                            throw new Exception("[0] Firefox configuration failed... Rolling back...");
                        }
                    }
                    else
                    {
                        Console.Out.WriteLine("Firefox is already configured. Skipping...");
                    }
                }
                else
                {
                    Console.WriteLine("64-bit Mozilla Firefox configuration failed. Missing dependency: 64-bit Mozilla Firefox is not installed.");
                }
            }
            catch
            {
                throw new Exception("[1] Firefox configuration failed... Rolling back...");
            }
        }

        public static bool IsSetupSuccessful()
        {
            bool isSuccessful = IsFirefoxInstalled() && IsFirefoxConfigured();
            return isSuccessful;
        }

        private static bool IsFirefoxInstalled()
        {
            bool isInstalled = ProgramInstaller.IsSoftwareInstalled(ProgramInstaller.Architectures.X64, "Mozilla Firefox * (x64 *)") == true;
            return isInstalled;
        }

        public static bool IsFirefoxConfigured()
        {
            bool isConfigured = 
                File.Exists(Path.Combine(SetupProperties.Firefoxinstall, @"browser\override.ini")) &&
                File.Exists(Path.Combine(SetupProperties.Firefoxinstall, @"defaults\pref\autoconfig.js")) &&
                File.Exists(Path.Combine(SetupProperties.Firefoxinstall, @"firefox.cfg"));

            return isConfigured;
        }
    }
}
