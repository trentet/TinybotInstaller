namespace TinybotInstaller.Tasks
{
    public enum ComponentStatus
    {
        NOT_STARTED,
        IN_PROGRESS,
        SKIPPED,
        PASSED,
        FAILED,
        PARTIAL_PASS
    }

    public enum ComponentStatusReason
    {
        PREVIOUSLY_COMPLETED,
        FAILED_PREQUISITE,
        CLEANUP_FAILED,
        SETUP_FAILED,
        SUCCESSFUL
    }

    class ComponentTask
    {
        public ComponentTask(
            SetupTask setupTask, 
            CleanupTask cleanupTask)
        {
            Setup = setupTask;
            Cleanup = cleanupTask;
            Status = ComponentStatus.NOT_STARTED;
        }

        public ComponentStatus Status { get; set; }
        public ComponentStatusReason StatusReason { get; set; }
        public SetupTask Setup { get; set; }
        public CleanupTask Cleanup { get; set; }
        
        public void Execute()
        {
            Status = ComponentStatus.IN_PROGRESS;
            foreach(ValidationTask validationTask in Setup.SetupValidation)
            {
                validationTask.Task.Start();
            }

            if(Setup.SetupValidationResult() == true)
            {
                Status = ComponentStatus.SKIPPED;
            }
            else
            {
                foreach (ValidationTask validationTask in Setup.PrerequisiteValidation)
                {
                    validationTask.Task.Start();
                }

                if (Setup.PrerequisiteValidationResult() == true)
                {
                    Setup.Task.Start();
                    foreach (ValidationTask validationTask in Setup.SetupValidation)
                    {
                        validationTask.Task.Start();
                    }

                    if (Setup.SetupValidationResult() == true)
                    {
                        Cleanup.Task.Start();
                        Cleanup.CleanupValidation.Task.Start();
                        if (Cleanup.CleanupValidation.Task.Result == true)
                        {
                            Status = ComponentStatus.PASSED;
                            StatusReason = ComponentStatusReason.SUCCESSFUL;
                        }
                        else
                        {
                            Status = ComponentStatus.PARTIAL_PASS;
                            StatusReason = ComponentStatusReason.CLEANUP_FAILED;
                        }
                    }
                    else
                    {
                        Status = ComponentStatus.FAILED;
                        StatusReason = ComponentStatusReason.SETUP_FAILED;
                    }
                }
                else
                {
                    Status = ComponentStatus.FAILED;
                    StatusReason = ComponentStatusReason.FAILED_PREQUISITE;
                }
            }
        }
    }
}
