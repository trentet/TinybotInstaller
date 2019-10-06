using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TinybotInstaller.Tasks
{
    class SetupTask
    {
        private List<ValidationTask> prerequisiteValidationTasks = new List<ValidationTask>();
        private Task task;
        private List<ValidationTask> postSetupValidationTasks = new List<ValidationTask>();
        private bool force = false;

        public SetupTask(
            Func<bool> prerequisiteValidationAction, 
            Action setupAction,
            Func<bool> postSetupValidationAction,
            bool force)
        {
            this.prerequisiteValidationTasks.Add(new ValidationTask(prerequisiteValidationAction));
            this.task = new Task(setupAction);
            this.postSetupValidationTasks.Add(new ValidationTask(postSetupValidationAction));
            this.force = force;
        }

        public SetupTask(
            Func<bool> prerequisiteValidationAction,
            Action<object> setupAction,
            object setupActionParam,
            Func<bool> postSetupValidationAction,
            bool force)
        {
            this.prerequisiteValidationTasks.Add(new ValidationTask(prerequisiteValidationAction));
            this.task = new Task(setupAction, setupActionParam);
            this.postSetupValidationTasks.Add(new ValidationTask(postSetupValidationAction));
            this.force = force;
        }

        public SetupTask(
            List<Func<bool>> prerequisiteValidationActions,
            Action setupAction,
            List<Func<bool>> postSetupValidationActions,
            bool force)
        {
            foreach(Func<bool> prerequisiteValidationAction in prerequisiteValidationActions)
            {
                this.prerequisiteValidationTasks.Add(new ValidationTask(prerequisiteValidationAction));
            }
            this.task = new Task(setupAction);
            foreach (Func<bool> postSetupValidationAction in postSetupValidationActions)
            {
                this.postSetupValidationTasks.Add(new ValidationTask(postSetupValidationAction));
            }
            this.force = force;
        }

        public SetupTask(
            List<Func<bool>> prerequisiteValidationActions,
            Action<object> setupAction,
            object setupActionParam,
            List<Func<bool>> postSetupValidationActions,
            bool force)
        {
            foreach (Func<bool> prerequisiteValidationAction in prerequisiteValidationActions)
            {
                this.prerequisiteValidationTasks.Add(new ValidationTask(prerequisiteValidationAction));
            }
            this.task = new Task(setupAction, setupActionParam);
            foreach (Func<bool> postSetupValidationAction in postSetupValidationActions)
            {
                this.postSetupValidationTasks.Add(new ValidationTask(postSetupValidationAction));
            }
            this.force = force;
        }

        public SetupTask(
            List<Func<bool>> prerequisiteValidationActions,
            Action<object> setupAction,
            object setupActionParam,
            Func<bool> postSetupValidationAction,
            bool force)
        {
            foreach (Func<bool> prerequisiteValidationAction in prerequisiteValidationActions)
            {
                this.prerequisiteValidationTasks.Add(new ValidationTask(prerequisiteValidationAction));
            }
            this.task = new Task(setupAction, setupActionParam);
            this.postSetupValidationTasks.Add(new ValidationTask(postSetupValidationAction));
            this.force = force;
        }

        public SetupTask(
            Func<bool> prerequisiteValidationAction,
            Action<object> setupAction,
            object setupActionParam,
            List<Func<bool>> postSetupValidationActions,
            bool force)
        {
            this.prerequisiteValidationTasks.Add(new ValidationTask(prerequisiteValidationAction));
            this.task = new Task(setupAction, setupActionParam);
            foreach (Func<bool> postSetupValidationAction in postSetupValidationActions)
            {
                this.postSetupValidationTasks.Add(new ValidationTask(postSetupValidationAction));
            }
            this.force = force;
        }

        public List<ValidationTask> PrerequisiteValidation { get => prerequisiteValidationTasks; }
        public Task Task { get => task; } 
        public List<ValidationTask> SetupValidation { get => postSetupValidationTasks; }
        
        public bool PrerequisiteValidationResult()
        {
            foreach(ValidationTask validationTask in prerequisiteValidationTasks)
            {
                if(validationTask.Task.Result == false)
                {
                    return false;
                }
            }

            return true;
        }

        public bool SetupValidationResult()
        {
            foreach (ValidationTask validationTask in postSetupValidationTasks)
            {
                if (validationTask.Task.Result == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
