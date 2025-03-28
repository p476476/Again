namespace Again.Runtime.Commands.Spine
{
    public class ScaleSpineCommand : Command, IScalableCommand
    {
        public string Name { get; set; }
        public float Duration { get; set; } = 1f;
        
        public float ScaleX { get; set; } = 1f;
        public float ScaleY { get; set; } = 1f;

        public float AnchorX { get; set; } = 0.5f;
        public float AnchorY { get; set; } = 0.5f;

        public override void Execute()
        {
            var spineManager = AgainSystem.Instance.SpineManager;
            spineManager.Scale(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}