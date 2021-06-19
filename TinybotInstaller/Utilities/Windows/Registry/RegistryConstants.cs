using System.IO;

namespace TinybotInstaller
{
    class RegistryConstants
    {
        public const string RUN_KEY = "Run";
        public const string RUN_ONCE_KEY = "RunOnce";
        public const string HKLM_WINDOWSCURRENTVERSION_PATH = @"HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion";
        public const string HKCU_WINDOWSCURRENTVERSION_PATH = @"HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion";
        public static readonly string HKLM_RUN_PATH = Path.Combine(HKLM_WINDOWSCURRENTVERSION_PATH, RUN_KEY);
        public static readonly string HKLM_RUN_ONCE_PATH = Path.Combine(HKLM_WINDOWSCURRENTVERSION_PATH, RUN_ONCE_KEY);
        public static readonly string HKCU_RUN_PATH = Path.Combine(HKCU_WINDOWSCURRENTVERSION_PATH, RUN_KEY);
        public static readonly string HKCU_RUN_ONCE_PATH = Path.Combine(HKCU_WINDOWSCURRENTVERSION_PATH, RUN_ONCE_KEY);
        public const string HKLM_WINDOWSNTCURRENTVERSION_PATH = @"HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion";
        public const string TINYBOT_VERSION_KEY = "CurrentVersionTinybot";
        public static readonly string TINYBOT_VERSION_KEY_PATH = Path.Combine(HKLM_WINDOWSNTCURRENTVERSION_PATH, TINYBOT_VERSION_KEY);
    }
}
