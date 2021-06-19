using System;
using System.IO;

namespace TinybotInstaller
{
    class SystemPathConstants
    {
        public static string StartupPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"\Microsoft\Windows\Start Menu\Programs\Startup\");
        public static string WindowsPath { get; } = @"C:\Windows\";
        public static string CmdPath { get; set; } = @"C:\Windows\System32\cmd.exe";
        public static string CmdPathCommand { get; set; } = (CmdPath + " /c ");
    }
}
