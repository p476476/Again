namespace Again.Scripts.Runtime.Commands
{
    public class HideDialogueCommand : Command

    {
        public override void Execute()
        {
            var againSystem = AgainSystem.Instance;
            var dialogueManager = againSystem.DialogueManager;
            dialogueManager.Hide();
            againSystem.NextCommand();
        }
    }
}