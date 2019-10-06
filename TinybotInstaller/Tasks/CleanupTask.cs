using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinybotInstaller.Tasks
{
    class CleanupTask
    {
        private Task task;
        private ValidationTask cleanupValidationTask;

        public CleanupTask(Action cleanupAction, Func<bool> cleanupValidationAction)
        {
            this.task = new Task(cleanupAction);
            this.cleanupValidationTask = new ValidationTask(cleanupValidationAction);
        }

        public CleanupTask(Action<object> cleanupAction, object obj, Func<bool> cleanupValidationAction)
        {
            this.task = new Task(cleanupAction, obj);
            this.cleanupValidationTask = new ValidationTask(cleanupValidationAction);
        }

        public Task Task { get => task; }
        public ValidationTask CleanupValidation { get => cleanupValidationTask; }

    }
}
