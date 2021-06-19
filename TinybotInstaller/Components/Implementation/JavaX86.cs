using System;
using TinybotInstaller.Tasks;

namespace TinybotInstaller.Components
{
    class JavaX86 : Component
    {
        public JavaX86() : base("Java 8 32-bit")
        {
            ComponentTasks.Add(1, JavaX86Installation());
        }

        private ComponentTask JavaX86Installation()
        {
            Func<bool> isChocoInstalled =
               () => ChocoUtil.IsChocoInstalled();

            /* Update to use non-default argument list to specifically get x86 version */
            Action installJavaX86 =
                () => ComponentTaskPresets.InstallWithChoco(Architectures.X86, "Java 8 (32-bit)", "jre8", "Java 8 Update *$", false);

            Func<bool> isX86Java8Installed =
               () => JavaInstallUtil.IsJavaInstalled(Architectures.X86, 8);

            return new ComponentTask(
                new SetupTask(
                    isChocoInstalled,
                    installJavaX86,
                    isX86Java8Installed,
                    false),
                null);
        }
    }
}
