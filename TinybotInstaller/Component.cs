using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinybotInstaller
{
    class Component
    {
        private string name;
        private object componentType;
        private List<object> tasks = new List<object>();

        public bool VerifyPrerequisites()
        {

            return false;
        }

        public void ExecuteTasks()
        {
            
        }

        public bool VerifyTaskCompletion()
        {

            return false;
        }
    }
}
