using Again.Runtime.Enums;

namespace Again.Runtime.Commands.Image
{
    public class HideImageCommand : Command
    {
        public string Name { get; set; }

        public HideAnimationType HideType { get; set; } = HideAnimationType.Fade;

        public float Duration { get; set; } = 1f;

        public override void Execute()
        {
            var imageManager = AgainSystem.Instance.ImageManager;
            imageManager.Hide(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}