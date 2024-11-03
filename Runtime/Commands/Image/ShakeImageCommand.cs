using Again.Runtime.Enums;

namespace Again.Runtime.Commands.Image
{
    public class ShakeImageCommand : Command
    {
        public string Name { get; set; }
        public float Duration { get; set; } = 1f;
        public float Strength { get; set; } = 30f;
        public int Vibrato { get; set; } = 6;
        public float Randomness { get; set; } = 90f;
        public bool Snapping { get; set; } = false;
        public bool FadeOut { get; set; } = false;
        public ShakeType ShakeType { get; set; } = ShakeType.HorizontalAndVertical;

        public override void Execute()
        {
            var imageManager = AgainSystem.Instance.ImageManager;
            imageManager.Shake(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}