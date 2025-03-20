namespace Again.Runtime.Commands
{
    public class HideBackgroundCommand:Command
    {

        public float Duration { get; set; } = 1f;
        
        public override void Execute()
        {
            AgainSystem.Instance.ImageManager.HideBackground(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}