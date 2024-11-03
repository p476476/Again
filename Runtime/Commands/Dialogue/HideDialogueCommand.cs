namespace Again.Runtime.Commands.Dialogue
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