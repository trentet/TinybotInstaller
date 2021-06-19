using System;
using System.Diagnostics;
using System.IO;
using TinybotInstaller.Tasks;

namespace TinybotInstaller.Components
{
    class OSRS : Component
    {
        public OSRS() : base("Old School RuneScape")
        {
            ComponentTasks.Add(1, OSRSInstallation());
        }

        private ComponentTask OSRSInstallation()
        {
            //Setup
            Func<bool> prequisiteFunc = null;
            Action action = () => SetupOSRS();
            Func<bool> actionValidation = () => VerifyOSRS();

            return new ComponentTask(new SetupTask(prequisiteFunc, action, actionValidation, false), null);
        }

        public void SetupOSRS()
        {
            if (VerifyOSRS() == false)
            {
                Console.Out.WriteLine("Installing OSRS client...");
                //Start - Job - ScriptBlock scriptBlock - ArgumentList @(90, "Old School RuneScape", "InstallOSRS") | Out - Null
                ProcessStartInfo start_info = new ProcessStartInfo(Path.Combine(SetupProperties.Rs, "InstallOSRS.exe"));
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
        }

        public static bool VerifyOSRS()
        {
            if (File.Exists(@"C:\Users\Administrator\jagexcache\jagexlauncher\bin\JagexLauncher.exe") 
                && File.Exists(@"C:\Users\Administrator\jagexcache\jagexlauncher\oldschool\oldschool.prm"))
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
