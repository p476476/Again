namespace Again.Runtime.Commands.OptionMenu
{
    public class OptionCommand : Command
    {
        public string Text { get; set; }

        public string Key { get; set; }
        public Command NextCommand { get; set; }
        public Command EndCommand { get; set; }

        public override void Execute()
        {
            AgainSystem.Instance.GoToCommand(EndCommand);
        }
    }
}