using TinybotInstaller.Tasks;

namespace TinybotInstaller.Components
{
    class WinRAR : Component
    {
        public WinRAR() : base("WinRAR")
        {
            ComponentTasks.Add(1, WinRARInstallation());
        }

        private ComponentTask WinRARInstallation()
        {
            return ComponentTaskPresets.InstallWithChoco(Architectures.X64, "WinRAR", "winrar", "WinRAR *.** (64-bit)", false);
        }
    }
}
