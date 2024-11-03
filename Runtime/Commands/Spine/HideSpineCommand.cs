using Again.Runtime.Enums;

namespace Again.Runtime.Commands.Spine
{
    public class HideSpineCommand : Command
    {
        public string Name { get; set; }

        public HideAnimationType HideType { get; set; } = HideAnimationType.Fade;

        public float Duration { get; set; } = 1f;

        public override void Execute()
        {
            var spineManager = AgainSystem.Instance.SpineManager;
            spineManager.Hide(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}