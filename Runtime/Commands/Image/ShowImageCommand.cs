using Again.Scripts.Runtime.Enums;

namespace Again.Scripts.Runtime.Commands.Image
{
    public class ShowImageCommand : Command
    {
        public string Name { get; set; }

        public string ImageName { get; set; }

        public ShowAnimationType ShowType { get; set; } = ShowAnimationType.Fade;

        public float Duration { get; set; } = 1f;

        public float PosX { get; set; } = 0;

        public float PosY { get; set; } = 0;

        public float Scale { get; set; } = 1f;


        public override void Execute()
        {
            var imageManager = AgainSystem.Instance.ImageManager;
            imageManager.Show(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}