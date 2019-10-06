using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinybotInstaller.Tasks
{
    class ValidationTask
    {
        private Task<bool> task;

        public ValidationTask(Func<bool> taskAction)
        {
            this.task = new Task<bool>(taskAction);
        }
        
        public Task<bool> Task { get => task; }
    }
}
