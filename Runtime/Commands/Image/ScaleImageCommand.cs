namespace Again.Scripts.Runtime.Commands.Image
{
    public class ScaleImageCommand : Command
    {
        public string Name { get; set; }
        public float Duration { get; set; } = 1f;
        public float Scale { get; set; } = 1f;

        public float AnchorX { get; set; } = 0.5f;
        public float AnchorY { get; set; } = 0.5f;

        public override void Execute()
        {
            var imageManager = AgainSystem.Instance.ImageManager;
            imageManager.Scale(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}