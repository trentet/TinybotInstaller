using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinybotInstaller.Tasks;

namespace TinybotInstaller.Components
{
    class RS3 : Component
    {
        public RS3() : base("RuneScape 3")
        {
            ComponentTasks.Add(1, RS3Installation());
        }

        private ComponentTask RS3Installation()
        {
            //Setup
            Func<bool> prequisiteFunc = null;
            Action action = () => InstallRS3();
            Func<bool> actionValidation = () => VerifyRS3();

            //Cleanup
            Action cleanupAction = () => CleanupRS3Install();
            Func<bool> cleanupActionValidation = () => ValidateCleanupRS3Install();

            return new ComponentTask(
                new SetupTask(prequisiteFunc, action, actionValidation, false), 
                new CleanupTask(cleanupAction, cleanupActionValidation));
        }

        public static void InstallRS3()
        {
            if (VerifyRS3() == false)
            {
                Console.Out.WriteLine("Installing RS3 client...");
                //Start - Job - ScriptBlock scriptBlock - ArgumentList @(90, "RuneScape 3", "InstallRS3") | Out - Null
                ProcessStartInfo start_info = new ProcessStartInfo(Path.Combine(SetupProperties.Rs, "InstallRS3.exe"));
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
        }

        public void CleanupRS3Install()
        {
            if (Directory.Exists(SetupProperties.Rs))
            {
                Directory.Delete(SetupProperties.Rs, true);
            }
        }

        public bool ValidateCleanupRS3Install()
        {
            return Directory.Exists(SetupProperties.Rs);
        }

        public static bool VerifyRS3()
        {
             if (File.Exists(@"C:\Users\Administrator\jagexcache\jagexlauncher\bin\JagexLauncher.exe") 
                && File.Exists(@"C:\Users\Administrator\jagexcache\jagexlauncher\runescape\runescape.prm"))
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
