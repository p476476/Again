namespace Again.Runtime.Commands.Spine
{
    public class JumpSpineCommand : Command
    {
        public string Name { get; set; }
        public float Duration { get; set; } = 0.5f;
        public float JumpPower { get; set; } = 100f;
        public int JumpCount { get; set; } = 1;

        public override void Execute()
        {
            var spineManager = AgainSystem.Instance.SpineManager;
            spineManager.Jump(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}