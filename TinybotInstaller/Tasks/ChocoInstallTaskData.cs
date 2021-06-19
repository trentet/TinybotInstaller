namespace TinybotInstaller.Tasks
{
    class ChocoInstallData
    {
        public Architectures Architecture { get; set; }
        public string DisplayName { get; set; }
        public string PackageName { get; set; }
        public string SearchMask { get; set; }

        public ChocoInstallData(
            Architectures architecture,
            string displayName,
            string pkgName,
            string searchMask)
        {
            Architecture = architecture;
            DisplayName = displayName;
            PackageName = pkgName;
            SearchMask = searchMask;
        }
    }
}
