using Microsoft.Win32;
using System;
using System.Collections.Generic;

namespace TinybotInstaller
{
    class JavaInstallUtil
    {
        public static List<Version> GetJavaJREVersions(List<RegistryKey> keys)
        {
            List<Version> versions = new List<Version>();
            foreach (RegistryKey key in keys)
            {
                string currentVersion = key.GetValue("CurrentVersion").ToString(); ;
                var subKeyNames = key.GetSubKeyNames();

                foreach (string subKeyName in subKeyNames)
                {
                    if (!subKeyName.Equals(currentVersion))
                    {
                        string versionText = key.OpenSubKey(subKeyName + "\\MSI").GetValue("PRODUCTVERSION").ToString();
                        versions.Add(new Version(versionText));
                    }
                }
            }

            return versions;
        }

        public static List<Version> Get32bitJavaJREVersions()
        {
            List<RegistryKey> x86JavaJREKeys = new List<RegistryKey>();

            //JRE 8 and older
            x86JavaJREKeys.Add(Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\JavaSoft\\Java Runtime Environment"));

            //There are no 32-bit JRE installs for versions 9 and new

            List<Version> versions = GetJavaJREVersions(x86JavaJREKeys);
            return versions;
        }

        public static List<Version> Get64bitJavaJREVersions()
        {
            List<RegistryKey> x64JavaJREKeys = new List<RegistryKey>();

            //JRE 8 and older
            x64JavaJREKeys.Add(Registry.LocalMachine.OpenSubKey("SOFTWARE\\JavaSoft\\Java Runtime Environment"));

            //JRE 9
            x64JavaJREKeys.Add(Registry.LocalMachine.OpenSubKey("SOFTWARE\\JavaSoft\\JRE"));

            //No JRE distributed after version 9. User either must have full JDK installed, or the application developer must jlink a minimum runtime version

            List<Version> versions = GetJavaJREVersions(x64JavaJREKeys);
            return versions;
        }

        public static List<Version> GetJavaJDKVersions(List<RegistryKey> keys)
        {
            List<Version> versions = new List<Version>();
            foreach (RegistryKey key in keys)
            {
                var subKeyNames = key.GetSubKeyNames();
                foreach (string subKeyName in subKeyNames)
                {
                    versions.Add(new Version(subKeyName));
                }
            }

            return versions;
        }

        public static List<Version> Get32bitJavaJDKVersions()
        {
            List<RegistryKey> x86JavaJDKKeys = new List<RegistryKey>
            {
                //JDK 8 and older
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\JavaSoft\\Java Development Kit")
            };

            List<Version> versions = GetJavaJDKVersions(x86JavaJDKKeys);
            return versions;
        }

        public static List<Version> Get64bitJavaJDKVersions()
        {
            List<RegistryKey> x64JavaJDKKeys = new List<RegistryKey>
            {
                //JDK 8 and older
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\JavaSoft\\Java Development Kit"),

                //JDK 9 and newer
                Registry.LocalMachine.OpenSubKey("SOFTWARE\\JavaSoft\\JDK")

                //No 32-bit version of JDK distributed after version 9
            };

            List<Version> versions = GetJavaJDKVersions(x64JavaJDKKeys);
            return versions;
        }

        public static bool IsJavaInstalled(Architectures architecture, int majorVersion)
        {
            bool isInstalled = false;

            if (architecture == Architectures.X86)
            {
                if (Get32bitJavaJREVersions().FindIndex(item => item.Major == majorVersion) >= 0
                    || ProgramInstaller.IsSoftwareInstalled(Architectures.X86, "Java " + majorVersion + " Update *"))
                {
                    isInstalled = true;
                }
            }
            else if (architecture == Architectures.X64)
            {
                if (Get64bitJavaJREVersions().FindIndex(item => item.Major == majorVersion) >= 0
                    || ProgramInstaller.IsSoftwareInstalled(Architectures.X64, "Java " + majorVersion + " Update * (64-bit)"))
                {
                    isInstalled = true;
                }
            }
            else
            {
                throw new InvalidOperationException("Invalid architecture");
            }

            return isInstalled;
        }
    }
}
