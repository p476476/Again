using Again.Runtime.Enums;

namespace Again.Runtime.Commands.Spine
{
    public class ShowSpineCommand : Command, IScalableCommand
    {
        public string Name { get; set; }
        public string SpineName { get; set; }
        public string Animation { get; set; }
        public string Skin { get; set; }

        public ShowAnimationType ShowType { get; set; } = ShowAnimationType.Fade;

        public float Duration { get; set; } = 1f;

        public float PosX { get; set; } = 0;

        public float PosY { get; set; } = 0;

        public float ScaleX { get; set; } = 1f;
        
        public float ScaleY { get; set; } = 1f;

        public bool IsLoop { get; set; } = true;

        public int Order { get; set; } = (int)Enums.Order.Image;

        public override void Execute()
        {
            var spineManager = AgainSystem.Instance.SpineManager;
            spineManager.Show(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}