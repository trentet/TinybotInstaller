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
        public enum RegistryHives
        {
            CLASSES_ROOT = 0,
            CURRENT_USER = 1,
            LOCAL_MACHINE = 2,
            USERS = 3,
            CURRENT_CONFIG = 4,
            PERFORMANCE_DATA = 5
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

        public static bool RegistryKeyValueDataExists(RegistryKey key, string keyValueName, string desiredKeyValueData)
        {
            bool keyValueDataExists = false;
            try
            {
                var keyValue = key.GetValue(keyValueName);
                if (keyValue != null)
                {
                    var keyValueData = string.Join("", (string[])keyValue);
                    if (keyValueData.Equals(desiredKeyValueData))
                    {
                        keyValueDataExists = true;
                    }
                }
                
                return keyValueDataExists;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string TrimRegistryHive(string registryPath)
        {
            string registryHivePrefix = registryPath.Split(new[] { '\\' }, 2)[0];

            for (int x = 0; x < RegistryHiveDict.Count; x++)
            {
                if (registryHivePrefix.Equals(RegistryHiveDict[(RegistryHives)x].Name))
                {
                    registryPath = registryPath.Substring(RegistryHiveDict[(RegistryHives)x].Name.Length);
                }
                else if (registryHivePrefix.Equals(RegistryHiveDict[(RegistryHives)x].AbrvName))
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

                if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CLASSES_ROOT].Name) || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CLASSES_ROOT].AbrvName))
                {
                    selectedRegHiveIndex = (int)RegistryHives.CLASSES_ROOT;
                }
                else if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CURRENT_USER].Name) || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CURRENT_USER].AbrvName))
                {
                    selectedRegHiveIndex = (int)RegistryHives.CURRENT_USER;
                }
                else if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.LOCAL_MACHINE].Name) || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.LOCAL_MACHINE].AbrvName))
                {
                    selectedRegHiveIndex = (int)RegistryHives.LOCAL_MACHINE;
                }
                else if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.USERS].Name) || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.USERS].AbrvName))
                {
                    selectedRegHiveIndex = (int)RegistryHives.USERS;
                }
                else if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CURRENT_CONFIG].Name) || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.CURRENT_CONFIG].AbrvName))
                {
                    selectedRegHiveIndex = (int)RegistryHives.CURRENT_CONFIG;
                }
                else if (registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.PERFORMANCE_DATA].Name) || registryPathPieces[0].Equals(RegistryHiveDict[RegistryHives.PERFORMANCE_DATA].AbrvName))
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
            catch (Exception ex)  //just for demonstration...it's always best to handle specific exceptions
            {
                throw ex;
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
