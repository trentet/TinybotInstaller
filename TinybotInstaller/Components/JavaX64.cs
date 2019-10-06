using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinybotInstaller.Tasks;

namespace TinybotInstaller.Components
{
    class JavaX64 : Component
    {
        public JavaX64() : base("Java 8 64-bit")
        {
            ComponentTasks.Add(1, JavaX64Installation());
        }

        private ComponentTask JavaX64Installation()
        {
            /* Update to use non-default argument list to specifically get x64 version */
            return ComponentTaskPresets.InstallWithChoco(Architectures.X64, "Java 8 (64-bit)", "jre8", "Java 8 Update * (64-bit)", false);
        }
    }
}
