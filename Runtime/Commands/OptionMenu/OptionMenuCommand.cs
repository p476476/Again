using System.Collections.Generic;

namespace Again.Runtime.Commands.OptionMenu
{
    public class OptionMenuCommand : Command
    {
        public List<OptionCommand> Options { get; set; } = new();

        public override void Execute()
        {
            var dialogueManager = AgainSystem.Instance.DialogueManager;
            dialogueManager.ShowOptionMenu(this,
                index => { AgainSystem.Instance.GoToCommand(Options[index].NextCommand); });
        }
    }
}