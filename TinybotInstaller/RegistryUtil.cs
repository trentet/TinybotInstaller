using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinybotInstaller
{
    static class RegistryUtil
    {
        public static RegistryKey HKLM_Run_Key = OpenSubKey(RegistryHives.LOCAL_MACHINE, RegistryConstants.HKLM_RUN_PATH);
        public static RegistryKey HKLM_RunOnce_Key = OpenSubKey(RegistryHives.LOCAL_MACHINE, RegistryConstants.HKLM_RUN_ONCE_PATH);
        public static RegistryKey HKCU_Run_Key = OpenSubKey(RegistryHives.LOCAL_MACHINE, RegistryConstants.HKLM_RUN_PATH);
        public static RegistryKey HKCU_RunOnce_Key = OpenSubKey(RegistryHives.LOCAL_MACHINE, RegistryConstants.HKLM_RUN_ONCE_PATH);

        public enum RegistryHives
        {
            CLASSES_ROOT = 0,
            CURRENT_USER = 1,
            LOCAL_MACHINE = 2,
            USERS = 3,
            CURRENT_CONFIG = 4,
            PERFORMANCE_DATA = 5
        }

        public enum StartupRegistryHives
        {
            CURRENT_USER = 1,
            LOCAL_MACHINE = 2
        }

        public static Dictionary<RegistryHives, RegistryHive> RegistryHiveDict = new Dictionary<RegistryHives, RegistryHive>
        {
            {RegistryHives.CLASSES_ROOT, new RegistryHive("HKEY_CLASSES_ROOT", "HKCR:")},
            {RegistryHives.CURRENT_USER,  new RegistryHive("HKEY_CURRENT_USER", "HKCU:")},
            {RegistryHives.LOCAL_MACHINE,  new RegistryHive("HKEY_LOCAL_MACHINE", "HKLM:")},
            {RegistryHives.USERS,  new RegistryHive("HKEY_USERS", "HHKU:")},
            {RegistryHives.CURRENT_CONFIG,  new RegistryHive("HKEY_CURRENT_CONFIG", "HKCC:")},
            {RegistryHives.PERFORMANCE_DATA, new RegistryHive("HKEY_PERFORMANCE_DATA", "HKPD:")}
        };

        public static RegistryKey GetStartupRunKey(bool runOnce, StartupRegistryHives startupHive)
        {
            if (startupHive == StartupRegistryHives.CURRENT_USER)
            {
                if (runOnce) { return HKCU_RunOnce_Key; } else { return HKCU_Run_Key; }
            }
            else
            {
                if (runOnce) { return HKLM_RunOnce_Key; } else { return HKLM_Run_Key; }
            }
        }

        public static void SetRunCommand(
            bool runOnce, 
            StartupRegistryHives startupHive, 
            string keyValueName, 
            CLICommand command)
        {
            string registryKeyValueData = SystemPathConstants.CmdPathCommand + " " + command.ToString();
            RegistryKey startupRunKey = GetStartupRunKey(runOnce, startupHive);

            Registry.SetValue(startupRunKey.Name, keyValueName, registryKeyValueData, RegistryValueKind.String);
        }

        public static void SetRunCommand(
            bool runOnce,
            StartupRegistryHives startupHive,
            string keyValueName,
            CLICommand[] commands)
        {
            string registryKeyValueData = SystemPathConstants.CmdPathCommand;

            registryKeyValueData += CMDCommands.ToOneLine(commands);

            RegistryKey startupRunKey = GetStartupRunKey(runOnce, startupHive);

            Registry.SetValue(startupRunKey.Name, keyValueName, registryKeyValueData, RegistryValueKind.String);
        }

        public static void SetRunCommand(
            bool runOnce,
            StartupRegistryHives startupHive,
            string keyValueName,
            CMDForLoopCommand command)
        {
            string registryKeyValueData = SystemPathConstants.CmdPathCommand + " " + command.ToString();
            RegistryKey startupRunKey = GetStartupRunKey(runOnce, startupHive);

            Registry.SetValue(startupRunKey.Name, keyValueName, registryKeyValueData, RegistryValueKind.String);
        }

        public static bool RegistryKeyValueDataExists(
            RegistryKey key, 
            string keyValueName, 
            string desiredKeyValueData,
            StringComparison stringComparison)
        {
            bool keyValueDataExists = false;
            try
            {
                var keyValue = key.GetValue(keyValueName);
                if (keyValue != null)
                {
                    var keyValueData = string.Join("", (string[])keyValue);
                    if (keyValueData.Equals(desiredKeyValueData, stringComparison))
                    {
                        keyValueDataExists = true;
                    }
                }
                
                return keyValueDataExists;
            }
            catch
            {
                throw;
            }
        }

        public static string TrimRegistryHive(string registryPath)
        {
            string registryHivePrefix = registryPath.Split(new[] { '\\' }, 2)[0];

            for (int x = 0; x < RegistryHiveDict.Count; x++)
            {
                if (registryHivePrefix.Equals(RegistryHiveDict[(RegistryHives)x].Name, StringComparison.OrdinalIgnoreCase))
                {
                    registryPath = registryPath.Substring(RegistryHiveDict[(RegistryHives)x].Name.Length);
                }
                else if (registryHivePrefix.Equals(RegistryHiveDict[(RegistryHives)x].AbrvName, StringComparison.OrdinalIgnoreCase))
                {
                    registryPath = registryPath.Substring(RegistryHiveDict[(RegistryHives)x].AbrvName.Length);
                }
            }

            registryPath = registryPath.Trim('\\');

            return registryPath;
        }

        public static RegistryHives? GetRegistryHive(string registryPath)
        {
            try
            {
                var registryPathPieces = registryPath.Split(new[] { '\\' }, 2);

                int selectedRegHiveIndex;

                if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CLASSES_ROOT].Name, StringComparison.OrdinalIgnoreCase) 
                    || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CLASSES_ROOT].AbrvName, StringComparison.OrdinalIgnoreCase))
                {
                    selectedRegHiveIndex = (int)RegistryHives.CLASSES_ROOT;
                }
                else if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CURRENT_USER].Name, StringComparison.OrdinalIgnoreCase) 
                    || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CURRENT_USER].AbrvName, StringComparison.OrdinalIgnoreCase))
                {
                    selectedRegHiveIndex = (int)RegistryHives.CURRENT_USER;
                }
                else if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.LOCAL_MACHINE].Name, StringComparison.OrdinalIgnoreCase) 
                    || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.LOCAL_MACHINE].AbrvName, StringComparison.OrdinalIgnoreCase))
                {
                    selectedRegHiveIndex = (int)RegistryHives.LOCAL_MACHINE;
                }
                else if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.USERS].Name, StringComparison.OrdinalIgnoreCase) 
                    || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.USERS].AbrvName, StringComparison.OrdinalIgnoreCase))
                {
                    selectedRegHiveIndex = (int)RegistryHives.USERS;
                }
                else if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CURRENT_CONFIG].Name, StringComparison.OrdinalIgnoreCase) 
                    || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CURRENT_CONFIG].AbrvName, StringComparison.OrdinalIgnoreCase))
                {
                    selectedRegHiveIndex = (int)RegistryHives.CURRENT_CONFIG;
                }
                else if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.PERFORMANCE_DATA].Name, StringComparison.OrdinalIgnoreCase) 
                    || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.PERFORMANCE_DATA].AbrvName, StringComparison.OrdinalIgnoreCase))
                {
                    selectedRegHiveIndex = (int)RegistryHives.PERFORMANCE_DATA;
                }
                else
                {
                    selectedRegHiveIndex = -1;
                }

                if (selectedRegHiveIndex == -1)
                {
                    return null;
                }
                else
                {
                    return (RegistryHives)selectedRegHiveIndex;
                }
            }
            catch //just for demonstration...it's always best to handle specific exceptions
            {
                throw;
            }
        }

        public static RegistryKey OpenSubKey(RegistryHives registryHive, string keyPath)
        {
            switch (registryHive)
            {
                case RegistryHives.CLASSES_ROOT:
                    return Registry.ClassesRoot.OpenSubKey(keyPath);
                case RegistryHives.CURRENT_USER:
                    return Registry.CurrentUser.OpenSubKey(keyPath);
                case RegistryHives.LOCAL_MACHINE:
                    return Registry.LocalMachine.OpenSubKey(keyPath);
                case RegistryHives.USERS:
                    return Registry.Users.OpenSubKey(keyPath);
                case RegistryHives.CURRENT_CONFIG:
                    return Registry.CurrentConfig.OpenSubKey(keyPath);
                case RegistryHives.PERFORMANCE_DATA:
                    return Registry.PerformanceData.OpenSubKey(keyPath);
                default:
                    throw new InvalidOperationException("Invalid Registry Hive Enum");
            }
        }
    }
}
