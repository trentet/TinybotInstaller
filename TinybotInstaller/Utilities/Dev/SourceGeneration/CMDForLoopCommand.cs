namespace TinybotInstaller
{
    class CMDForLoopCommand
    {
        private CommandOption forSwitch;
        private string loopParameter;
        private string inParameter;
        private CLICommand loopedCommand;

        public CommandOption ForSwitch { get; set; }
        public string LoopParameter { get; set; }
        public string InParameter { get; set; }
        public CLICommand LoopedCommand { get; set; }

        public CMDForLoopCommand()
        {
            
        }

        public CMDForLoopCommand(
            CommandOption forSwitch, 
            string loopParameter, 
            string inParameter, 
            CLICommand loopedCommand)
        {
            ForSwitch = forSwitch;
            LoopParameter = loopParameter;
            InParameter = inParameter;
            LoopedCommand = loopedCommand;
        }

        public override string ToString()
        {
            #pragma warning disable CA1305 // Specify IFormatProvider
            string forCommand = string.Format("for {0} {1} in ({2}) do {3}", ForSwitch.ToString(), LoopParameter, InParameter, LoopedCommand.ToString());
            #pragma warning restore CA1305 // Specify IFormatProvider

            return forCommand;
        }
    }
}
