using Again.Scripts.Runtime.Enums;

namespace Again.Scripts.Runtime.Commands.Spine
{
    public class ShowSpineCommand : Command
    {
        public string Name { get; set; }
        public string Animation { get; set; }
        public string Skin { get; set; }

        public ShowAnimationType ShowType { get; set; } = ShowAnimationType.Fade;

        public float Duration { get; set; } = 1f;

        public float PosX { get; set; } = 0;

        public float PosY { get; set; } = 0;

        public float Scale { get; set; } = 1f;

        public bool IsLoop { get; set; } = true;

        public override void Execute()
        {
            var spineManager = AgainSystem.Instance.SpineManager;
            spineManager.Show(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}