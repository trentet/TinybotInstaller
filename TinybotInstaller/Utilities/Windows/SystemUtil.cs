using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace TinybotInstaller
{
    public class Pointers
    {
        public IntPtr ImageOffset { get; set; }
        public IntPtr ImageBuffer { get; set; }
        public Pointers(IntPtr imageOffset, IntPtr imageBuffer)
        {
            ImageOffset = imageOffset;
            ImageBuffer = imageBuffer;
        }
    }
    public enum ActionIndexes
    {
        /* ref: http://windows10dll.nirsoft.net/shell32_dll.html
        * 5384 is the DLL index for "Adds this item to the Start Menu"
        * 5385 is the DLL index for "Removes this item from the Start Menu"
        * 5386 is the DLL index for "Pin to Taskbar"
        * 5387 is the DLL index for "Unpin to Taskbar"
        * This was original start menu index: //actionIndex = pin ? 51201 : 51394;
        */
        PIN_TO_TASKBAR = 5386,
        UNPIN_FROM_TASKBAR = 5387,
        PIN_TO_START_MENU = 5384,
        UNPIN_FROM_START_MENU = 5385,
    }

    public static class SystemUtil
    {
        [DllImport(
            "kernel32.dll",
            CharSet = CharSet.Auto, 
            SetLastError = true, 
            BestFitMapping = false, 
            ThrowOnUnmappableChar = true)]

        internal static extern IntPtr LoadLibrary(string lpLibFileName);

        [DllImport(
            "user32.dll", 
            CharSet = CharSet.Auto, 
            SetLastError = true, 
            BestFitMapping = false, 
            ThrowOnUnmappableChar = true)]

        internal static extern int LoadString(IntPtr hInstance, uint wID, StringBuilder lpBuffer, int nBufferMax);

        public static bool SetPin(ActionIndexes actionIndex, string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            int MAX_PATH = 255;

            StringBuilder szPinToStartLocalized = new StringBuilder(MAX_PATH);
            IntPtr hShell32 = LoadLibrary("Shell32.dll");
            LoadString(hShell32, (uint)actionIndex, szPinToStartLocalized, MAX_PATH);
            string localizedVerb = szPinToStartLocalized.ToString();

            string path = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);

            // create the shell application object
            dynamic shellApplication = Activator.CreateInstance(Type.GetTypeFromProgID("Shell.Application"));
            dynamic directory = shellApplication.NameSpace(path);
            dynamic link = directory.ParseName(fileName);

            dynamic verbs = link.Verbs();
            for (int i = 0; i < verbs.Count(); i++)
            {
                dynamic verb = verbs.Item(i);
                if (verb.Name.Equals(localizedVerb))
                {
                    verb.DoIt();
                    return true;
                }
            }
            return false;
        }

        //static string originalImagePathName;
        static int unicodeSize = IntPtr.Size * 2;

        static Pointers GetPointers()
        {
            IntPtr pebBaseAddress = GetBasicInformation().PebBaseAddress;
            var processParameters = Marshal.ReadIntPtr(pebBaseAddress, 4 * IntPtr.Size);
            IntPtr imageOffset = processParameters.Increment(4 * 4 + 5 * IntPtr.Size + unicodeSize + IntPtr.Size + unicodeSize);
            IntPtr imageBuffer = Marshal.ReadIntPtr(imageOffset, IntPtr.Size);
            return new Pointers(imageOffset, imageBuffer);
        }

        internal static string GetImagePathName()
        {
            //Read original data
            return GetImagePathName(GetPointers());
        }

        internal static string GetImagePathName(Pointers ptrs)
        {
            //Read original data
            var imageLen = Marshal.ReadInt16(ptrs.ImageOffset);
            string currentImagePathName = Marshal.PtrToStringUni(ptrs.ImageBuffer, imageLen / 2);
            return currentImagePathName;
        }

        internal static void ChangeImagePathName(string newFileName)
        {
            Pointers ptrs = GetPointers();
            ChangeImagePathName(newFileName, ptrs);
        }

        internal static void ChangeImagePathName(string newFileName, Pointers ptrs)
        {
            string originalImagePathName = GetImagePathName(ptrs);

            var newImagePathName = Path.Combine(Path.GetDirectoryName(originalImagePathName), newFileName);
            if (newImagePathName.Length > originalImagePathName.Length)
                throw new Exception("new ImagePathName cannot be longer than the original one");

            WriteImagePathName(newImagePathName, ptrs);
        }

        public static void WriteImagePathName(string imagePathName, Pointers ptrs)
        {
            //Write the string, char by char
            foreach (var unicodeChar in imagePathName)
            {
                Marshal.WriteInt16(ptrs.ImageBuffer, unicodeChar);
                ptrs.ImageBuffer = ptrs.ImageBuffer.Increment(2);
            }
            Marshal.WriteInt16(ptrs.ImageBuffer, 0);

            //Write the new length
            Marshal.WriteInt16(ptrs.ImageOffset, (short)(imagePathName.Length * 2));
        }

        /*internal static void RestoreImagePathName()
        {
            Pointers ptrs = GetPointers();

            WriteImagePathName(originalImagePathName, ptrs.ImageOffset, ptrs.ImageBuffer);
        }*/

        public static ProcessBasicInformation GetBasicInformation()
        {
            uint status;
            ProcessBasicInformation pbi;
            int retLen;
            var handle = System.Diagnostics.Process.GetCurrentProcess().Handle;
            if ((status = NtQueryInformationProcess(handle, 0,
                out pbi, Marshal.SizeOf(typeof(ProcessBasicInformation)), out retLen)) >= 0xc0000000)
                throw new Exception("Windows exception. status=" + status);
            return pbi;
        }

        [DllImport("ntdll.dll")]
        public static extern uint NtQueryInformationProcess(
            [In] IntPtr ProcessHandle,
            [In] int ProcessInformationClass,
            [Out] out ProcessBasicInformation ProcessInformation,
            [In] int ProcessInformationLength,
            [Out] [Optional] out int ReturnLength
            );

        public static IntPtr Increment(this IntPtr ptr, int value)
        {
            unchecked
            {
                if (IntPtr.Size == sizeof(Int32))
                    return new IntPtr(ptr.ToInt32() + value);
                else
                    return new IntPtr(ptr.ToInt64() + value);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ProcessBasicInformation
        {
            public uint ExitStatus;
            public IntPtr PebBaseAddress;
            public IntPtr AffinityMask;
            public int BasePriority;
            public IntPtr UniqueProcessId;
            public IntPtr InheritedFromUniqueProcessId;
        }

        public static bool IsFilePinned(string filePath)
        {
            // folder with shortcuts
            string location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar");
            if (!Directory.Exists(location))
                return false;

            foreach (var file in Directory.GetFiles(location, "*.lnk"))
            {
                IWshShell shell = new WshShell();
                var lnk = shell.CreateShortcut(file) as IWshShortcut;
                if (lnk != null)
                {
                    // if there is shortcut pointing to file path - it's pinned                                    
                    if (String.Equals(lnk.TargetPath, filePath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Returns whether the given path/file is a link
        /// </summary>
        /// <param name="shortcutFilename"></param>
        /// <returns></returns>
        public static bool IsLink(string shortcutFilename)
        {
            string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            Shell32.Shell shell = new Shell32.ShellClass();
            Shell32.Folder folder = shell.NameSpace(pathOnly);
            Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                return folderItem.IsLink;
            }
            return false; // not found
        }

        /// <summary>
        /// If path/file is a link returns the full pathname of the target,
        /// Else return the original pathnameo "" if the file/path can't be found
        /// </summary>
        /// <param name="shortcutFilename"></param>
        /// <returns></returns>
        public static string GetShortcutTarget(string shortcutFilename)
        {
            string pathOnly = System.IO.Path.GetDirectoryName(shortcutFilename);
            string filenameOnly = System.IO.Path.GetFileName(shortcutFilename);

            Shell32.Shell shell = new Shell32.ShellClass();
            Shell32.Folder folder = shell.NameSpace(pathOnly);
            Shell32.FolderItem folderItem = folder.ParseName(filenameOnly);
            if (folderItem != null)
            {
                if (folderItem.IsLink)
                {
                    Shell32.ShellLinkObject link = (Shell32.ShellLinkObject)folderItem.GetLink;
                    return link.Path;
                }
                return shortcutFilename;
            }
            return "";  // not found
        }

        public static string ExecuteCMDCommand(string command)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();

            cmd.StandardInput.WriteLine("echo Oscar");
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();

            return cmd.StandardOutput.ReadToEnd();
        }

        public static void CopyFolderContents(DirectoryInfo source, DirectoryInfo target, bool createDirectory)
        {
            if (createDirectory)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyFolderContents(diSourceSubDir, nextTargetSubDir, true);
            }
        }

        public static bool GetPendingReboot()
        {
            /*
             Check "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\" if a subkey named "RebootPending" is present. 
             Check "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\" if a subkey named "RebootRequired" is present. 
             Check "HKLM\SYSTEM\CurrentControlSet\Control\Session Manager\" if a value "PendingFileRenameOperations" if present. 
             If any of these conditions are met, a reboot is required.
            */

            bool hasPendingReboot = false;

            RegistryKey cbsKey = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing");
            RegistryKey wuauKey = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update");
            RegistryKey smssKey = RegistryUtil.OpenSubKey(RegistryUtil.RegistryHives.LOCAL_MACHINE, @"SYSTEM\CurrentControlSet\Control\Session Manager");

            if (cbsKey.OpenSubKey("RebootPending") != null || wuauKey.OpenSubKey("RebootRequired") != null || smssKey.GetValue("PendingFileRenameOperations") != null)
            {
                hasPendingReboot = true;
            }

            return hasPendingReboot;
        }
    }
}
