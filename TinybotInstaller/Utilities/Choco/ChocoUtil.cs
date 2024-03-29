﻿using Microsoft.Win32;
using System;
using System.IO;

namespace TinybotInstaller
{
    class ChocoUtil
    {
        public static string[] DefaultArgs { get; } = new string[] { "force", "y", "ignorechecksum" };
        public static string LocalChocolateyPackageFilePath { get; set; } = "";
        public static string Chocolatey { get; } = Path.Combine(SetupProperties.Install, @"Chocolatey\");
        public static string ChocolateyInstallScript { get; } = Path.Combine(Chocolatey, "InstallChocolatey.ps1");
        public static string ChocolateyLocalTemp { get; } = @"C:\Users\Administrator\AppData\Local\Temp\chocolatey";
        public static string ChocoInstallPath { get; } = Path.Combine(Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System)), @"ProgramData\Chocolatey\bin");

        public static bool IsChocoInstalled()
        {
            return File.Exists(ChocoInstallPath + "\\choco.exe");
        }

        public static void SetupChocolatey(string localChocolateyPackageFilePath)
        {
            //Start-Transaction -RollbackPreference Error
            try
            {
                Console.Out.WriteLine("Installing Chocolatey...");
                // Idempotence - do not install Chocolatey if it is already installed
                if (!IsChocoInstalled())
                {
                    string command = @"@powershell -NoProfile -ExecutionPolicy Bypass -Command ""iex ((new-object net.webclient).DownloadString('https://chocolatey.org/install.ps1'))"" && SET PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin";
                    SystemUtil.ExecuteCMDCommand(command);

                    if (IsChocoInstalled())
                    {
                        //Get - ChildItem localChocolateyPackageFilePath - Recurse - Force - ErrorAction SilentlyContinue | Remove - Item - Recurse - Force - Confirm:false;
                        //Get - ChildItem chocolatey - Recurse - Force - ErrorAction SilentlyContinue | Remove - Item - Recurse - Force - Confirm:false;
                        Console.Out.WriteLine("Configuring Chocolatey to update apps at startup...");
                        Registry.SetValue(RegistryConstants.HKLM_RUN_PATH, "Chocolatey", SystemPathConstants.CmdPathCommand + "cup all --y --ignorechecksum");
                        //CompleteTransaction();
                    }
                    else
                    {
                        //Console.Out.WriteLine(_.Exception.GetType().FullName, _.Exception.Message
                        //UndoTransaction();
                        //CancelSetup();
                        throw new Exception("[0] Chocolatey installation failed... Rolling back...");
                    }
                }
                else
                {
                    Console.Out.WriteLine("Chocolatey is already installed. Skipping...");
                    //CompleteTransaction();
                }
            }
            catch
            {
                //Console.Out.WriteLine(_.Exception.GetType().FullName, _.Exception.Message
                //UndoTransaction();
                //CancelSetup();
                throw new Exception("[1] Chocolatey installation failed... Rolling back...");
            }
        }

        public static bool ChocoUpgrade(string pkgName, string displayName, bool verifyInstall, string searchName, string[] argumentList)
        {
            try
            {
                Console.Out.WriteLine("Verifying network connectivity before installing " + displayName);
                if (Network.TestInternetConnection() == true)
                {
                    Console.Out.WriteLine("Network connectivity confirmed...");

                    if ((verifyInstall) == true)
                    {
                        Console.Out.WriteLine("Checking for pre-existing installs...");
                        var x86VersionExists = ProgramInstaller.IsSoftwareInstalled(Architectures.X86, searchName);
                        var x64VersionExists = ProgramInstaller.IsSoftwareInstalled(Architectures.X64, searchName);
                        Console.Out.WriteLine("32-bit version found?: " + x86VersionExists);
                        Console.Out.WriteLine("64-bit version found?: " + x64VersionExists);
                    }

                    Console.Out.WriteLine("Installing " + displayName + "...");

                    string arguments = "";
                    foreach (string argument in argumentList)
                    {
                        arguments += " --" + argument;
                    }

                    SystemUtil.ExecuteCMDCommand("choco.exe upgrade " + pkgName + arguments); //" --force --y --ignorechecksum ");

                    if ((verifyInstall) == true)
                    {
                        Console.Out.WriteLine("Verifying install...");
                        var x86VersionExists = ProgramInstaller.IsSoftwareInstalled(Architectures.X86, searchName);
                        var x64VersionExists = ProgramInstaller.IsSoftwareInstalled(Architectures.X64, searchName);
                        Console.Out.WriteLine("32-bit version found?: " + x86VersionExists);
                        Console.Out.WriteLine("64-bit version found?: " + x64VersionExists);
                        if (x64VersionExists == true || x86VersionExists == true)
                        {
                            return true;
                        }
                        else
                        {
                            Console.Out.WriteLine("[0] " + displayName + " installation failed... Rolling back...");
                            if (Network.TestInternetConnection() == false)
                            {
                                Console.Out.WriteLine("Failed to confirm network connectivity. if your host has connection, try: VMWare Toolbar -> Edit -> Virtual Network Editor -> Change Settings -> Restore Defaults. Then Restart");
                            }
                            return false;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    Console.Out.WriteLine("Failed to confirm network connectivity. if your host has connection, try: VMWare Toolbar -> Edit -> Virtual Network Editor -> Change Settings -> Restore Defaults. Then Restart");
                    return false;
                }
            }
            catch
            {
                Console.Out.WriteLine("[1] " + displayName + " installation failed... Rolling back...");
                return false;
            }
        }

    }
}
