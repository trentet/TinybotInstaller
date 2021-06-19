using TinybotInstaller.Tasks;

namespace TinybotInstaller.Components
{
    class TeamViewer : Component
    {
        public TeamViewer() : base("TeamViewer")
        {
            ComponentTasks.Add(1, TeamViewerInstallation());
        }

        private ComponentTask TeamViewerInstallation()
        {
            return ComponentTaskPresets.InstallWithChoco(Architectures.X86, "TeamViewer", "teamviewer", "TeamViewer **$", false);
        }
    }
}
