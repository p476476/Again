namespace Again.Runtime.Commands.Image
{
    public class MoveImageCommand : Command
    {
        public string Name { get; set; }
        public float Duration { get; set; } = 1f;
        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;

        public override void Execute()
        {
            var imageManager = AgainSystem.Instance.ImageManager;
            imageManager.Move(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}