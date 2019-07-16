using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinybotInstaller
{
    class RegistryHive
    {
        public string Name { get; set; }
        public string AbrvName { get; set; }

        public RegistryHive(string name, string abrvName)
        {
            this.Name = name;
            this.AbrvName = abrvName;
        }
    }
}
