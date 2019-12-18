using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinybotInstaller.Tasks;

namespace TinybotInstaller.Components
{
    class Chocolatey : Component
    {
        public Chocolatey() : base("Chocolatey")
        {
            ComponentTasks.Add(1, ChocolateyInstallation());
            ComponentTasks.Add(2, ChocolateyConfiguration());
        }

        private ComponentTask ChocolateyInstallation()
        {
            Func<bool> prequisiteFunc =
            () => Network.TestInternetConnection();

            Action action =
            () =>
            {
                string command = 
                    @"@powershell -NoProfile -ExecutionPolicy Bypass -Command ""iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))"" && SET PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin";
                SystemUtil.ExecuteCMDCommand(command);
            };

            Func<bool> actionValidation =
            () => ChocoUtil.IsChocoInstalled();

            return new ComponentTask(new SetupTask(prequisiteFunc, action, actionValidation, false), null);
        }

        private ComponentTask ChocolateyConfiguration()
        {

            Func<bool> prequisiteFunc =
            () => ComponentTasks.ElementAt(1).Value.Setup.SetupValidation.ElementAt(1).Task.Result;

            Action action =
            () =>
            {
                Console.Out.WriteLine("Configuring Chocolatey to update apps at startup...");
                Registry.SetValue(
                    RegistryConstants.HKLM_RUN_PATH, 
                    "Chocolatey", 
                    SystemPathConstants.CmdPathCommand + "cup all --y --ignorechecksum"
                );
            };

            Func<bool> actionValidation =
            () =>
            {
                bool configurationKeyValueExists = false;
                var chocolateyRunKeyValue = Registry.GetValue(RegistryConstants.HKLM_RUN_PATH, "Chocolatey", "");
                if(chocolateyRunKeyValue != null)
                {
                    configurationKeyValueExists = 
                        ((string)chocolateyRunKeyValue)
                            .Equals(SystemPathConstants.CmdPathCommand + "cup all --y --ignorechecksum");
                }               
                    
                return configurationKeyValueExists;
            };

            return new ComponentTask(new SetupTask(prequisiteFunc, action, actionValidation, false), null);
        }
    }
}
