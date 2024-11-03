namespace Again.Runtime.Commands.Image
{
    public class JumpImageCommand : Command
    {
        public string Name { get; set; }
        public float Duration { get; set; } = 0.5f;
        public float JumpPower { get; set; } = 100f;
        public int JumpCount { get; set; } = 1;

        public override void Execute()
        {
            var imageManager = AgainSystem.Instance.ImageManager;
            imageManager.Jump(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}