namespace Again.Runtime.Commands
{
    public class PlaySoundCommand : Command
    {
        public string SoundName { get; }

        public override void Execute()
        {
            AgainSystem.Instance.AudioManager.PlayAudio(SoundName);
        }
    }
}