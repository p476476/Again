namespace Again.Scripts.Runtime.Commands.OptionMenu
{
    public class OptionMenuEndCommand : Command
    {
        public override void Execute()
        {
            AgainSystem.Instance.NextCommand();
        }
    }
}