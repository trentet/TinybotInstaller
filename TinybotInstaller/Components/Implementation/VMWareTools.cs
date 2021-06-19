using TinybotInstaller.Tasks;

namespace TinybotInstaller.Components
{
    class VMWareTools : Component
    {
        public VMWareTools() : base("VMWareTools")
        {
            ComponentTasks.Add(1, VMWareToolsInstallation());
        }

        private ComponentTask VMWareToolsInstallation()
        {
            return ComponentTaskPresets.InstallWithChoco(Architectures.X64, "VMWare Tools", "vmware-tools", "VMWare Tools", false);
        }
    }
}
