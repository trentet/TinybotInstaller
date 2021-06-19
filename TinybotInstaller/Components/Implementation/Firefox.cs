using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using TinybotInstaller.Tasks;

namespace TinybotInstaller.Components
{
    class Firefox : Component
    {
        public Firefox() : base("Mozilla Firefox")
        {
            ComponentTasks.Add(1, FirefoxInstallation());
            ComponentTasks.Add(2, PinFirefoxToTaskbar());
            ComponentTasks.Add(3, FirefoxConfiguration());
        }

        private ComponentTask FirefoxInstallation()
        {
            return ComponentTaskPresets.InstallWithChoco(Architectures.X64, "Mozilla Firefox", "firefox", "Mozilla Firefox * (x64 *)", false);
        }

        private ComponentTask PinFirefoxToTaskbar()
        {
            return ComponentTaskPresets.PinProgramOrShortcutToTaskbar(@"C:\Program Files\Mozilla Firefox\firefox.exe");
        }

        private ComponentTask FirefoxConfiguration()
        {
            Func<bool> isFirefoxInstallSetupSuccessful =
                () => ComponentTasks.ElementAt(1).Value.Setup.SetupValidation.ElementAt(1).Task.Result;

            Action configureFirefox =
                () => 
                {
                    Console.Out.WriteLine("Copying Mozilla Firefox configuration files...");
                    File.Copy(
                        Path.Combine(SetupProperties.Firefox, @"browser\override.ini"), 
                        Path.Combine(SetupProperties.Firefoxinstall, @"browser\override.ini"), 
                        true);

                    File.Copy(
                        Path.Combine(SetupProperties.Firefox, @"defaults\pref\autoconfig.js"), 
                        Path.Combine(SetupProperties.Firefoxinstall, @"defaults\pref\autoconfig.js"), 
                        true);

                    File.Copy(
                        Path.Combine(SetupProperties.Firefox, @"firefox.cfg"), 
                        SetupProperties.Firefoxinstall, 
                        true);

                    Console.Out.WriteLine("Setting Mozilla Firefox as default browser...");
                    Process proc = new Process();
                    proc.StartInfo = new ProcessStartInfo(
                        Path.Combine(SetupProperties.Firefoxinstall, @"uninstall\helper.exe"),
                        "/SetAsDefaultAppGlobal");
                    proc.Start();
                    proc.WaitForExit();

                    proc.StartInfo = new ProcessStartInfo(
                        Path.Combine(SetupProperties.Firefoxinstall, @"uninstall\helper.exe"),
                        "/SetAsDefaultAppUser");
                    proc.Start();
                    proc.WaitForExit();
                };
            
            Func<bool> isFirefoxConfigured =
            () => File.Exists(Path.Combine(SetupProperties.Firefoxinstall, @"browser\override.ini")) 
               && File.Exists(Path.Combine(SetupProperties.Firefoxinstall, @"defaults\pref\autoconfig.js")) 
               && File.Exists(Path.Combine(SetupProperties.Firefoxinstall, @"firefox.cfg"));

            Action cleanupFirefoxConfiguration =
            () =>
            {
                if (Directory.Exists(SetupProperties.Firefox))
                {
                    Directory.Delete(SetupProperties.Firefox);
                }
            };

            Func<bool> isFirefoxConfigurationCleanupSuccessul =
            () => !Directory.Exists(SetupProperties.Firefox);

            return new ComponentTask(
                new SetupTask(isFirefoxInstallSetupSuccessful, configureFirefox, isFirefoxConfigured, false), 
                new CleanupTask(cleanupFirefoxConfiguration, isFirefoxConfigurationCleanupSuccessul));
        }
    }
}
