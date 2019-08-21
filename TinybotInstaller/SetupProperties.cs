using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinybotInstaller
{
    class SetupProperties
    {
        public static string ExecutionPath { get; set; } = "";
        public static string Setup { get; } = Path.Combine(ExecutionPath + @"\Setup\");
        public static string Config { get; } = Path.Combine(Setup, @"Config\");
        public static string Firefox { get; } = Path.Combine(Config + @"Firefox\");
        public static string Firefoxinstall { get; } = @"C:\Program Files\Mozilla Firefox\";
        public static string Os { get; } = Path.Combine(Config, @"OS\");
        public static string Oscopy { get; } = Path.Combine(Os, @"Copy\");
        public static string Pins { get; } = Path.Combine(Config, @"Pins\");
        public static string Install { get; } = Path.Combine(Setup + @"Install\");
        public static string Rs { get; } = Path.Combine(Install, @"RS\");
        public static List<string> CompatibleTinybotUpdgradeVersions { get; } = new List<string> { "4.4 Base 1" };
        public static Version NewTinybotVersion { get; } = new Version("5.0.0.0");
    }
}
