using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TinybotInstaller
{
    class ProgramInstaller
    {
        public enum Architectures
        {
            X86 = 1,
            X64 = 2,
            BOTH = 3
        }

        public static bool IsSoftwareInstalled(Architectures architecture, string softwareName, string remoteMachine = null, StringComparison strComparison = StringComparison.Ordinal)
        {
            List<string> uninstallRegKeys = new List<string>();
            if(architecture == Architectures.X86 || architecture == Architectures.BOTH)
            {
                uninstallRegKeys.Add(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            }
            if (architecture == Architectures.X64 || architecture == Architectures.BOTH)
            {
                uninstallRegKeys.Add(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            }

            bool isInstalled = false;
            foreach(string uninstallRegKey in uninstallRegKeys)
            {
                RegistryView[] enumValues = (RegistryView[])Enum.GetValues(typeof(RegistryView));

                //Starts from 1, because first one is Default, so we dont need it...
                for (int i = 1; i < enumValues.Length; i++)
                {
                    //This one key is all what we need, because RegView will do the rest for us
                    using (RegistryKey regKey = (string.IsNullOrWhiteSpace(remoteMachine))
                                ? RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, enumValues[i]).OpenSubKey(uninstallRegKey)
                                : RegistryKey.OpenRemoteBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, remoteMachine, enumValues[i]).OpenSubKey(uninstallRegKey))
                    {
                        if (SearchSubKeysForValue(regKey, "DisplayName", softwareName, strComparison).Result)
                        { isInstalled = true; }
                    }
                }
            }

            return isInstalled;
        }

        //This one does't have a case sensitive/insensitive option, but if you need it, just don't use LIKE %softwareName%
        //and get all products (SELECT Name FROM Win32_Product). After that just go through the result and compare...
        public static bool IsSoftwareInstalledWMI(string softwareName, string remoteMachine = null)
        {
            string wmiPath = (!string.IsNullOrEmpty(remoteMachine))
                                ? @"\\" + remoteMachine + @"\root\cimv2"
                                : @"\\" + Environment.MachineName + @"\root\cimv2";

            SelectQuery select = new SelectQuery(string.Format("SELECT * FROM Win32_Product WHERE Name LIKE \"%{0}%\"", softwareName));

            if (SelectStringsFromWMI(select, new ManagementScope(wmiPath)).Count > 0) { return true; }

            return false;
        }

        public static List<Dictionary<string, string>> SelectStringsFromWMI(SelectQuery select, ManagementScope wmiScope)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiScope, select))
            {
                using (ManagementObjectCollection objectCollection = searcher.Get())
                {
                    foreach (ManagementObject managementObject in objectCollection)
                    {
                        //With every new object we add new Dictionary
                        result.Add(new Dictionary<string, string>());
                        foreach (PropertyData property in managementObject.Properties)
                        {
                            //Always add data to newest Dictionary
                            result.Last().Add(property.Name, property.Value?.ToString());
                        }
                    }

                    return result;
                }
            }
        }

        public static async Task<bool> SearchSubKeysForValue(RegistryKey regKey, string valueName, string searchedValue, StringComparison strComparison = StringComparison.Ordinal)
        {
            string pattern = Regex.Escape(searchedValue).Replace("\\*", ".*?");

            bool result = false;
            string[] subKeysNames = regKey.GetSubKeyNames();
            List<Task<bool>> tasks = new List<Task<bool>>();

            for (int i = 0; i < subKeysNames.Length - 1; i++)
            {
                //We have to save current value for i, because we cannot use it in async task due to changed values for it during foor loop
                string subKeyName = subKeysNames[i];
                tasks.Add(Task.Run(() =>
                {
                    string value = regKey.OpenSubKey(subKeyName)?.GetValue(valueName)?.ToString() ?? null;
                    //return (value != null && value.IndexOf(searchedValue, strComparison) >= 0);
                    return (value != null && Regex.IsMatch(searchedValue, pattern));
                }));
            }

            bool[] results = await Task.WhenAll(tasks).ConfigureAwait(false);
            result = results.Contains(true);

            return result;
        }
    }
}
