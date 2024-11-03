namespace Again.Runtime.Commands.Spine
{
    public class MoveSpineCommand : Command
    {
        public string Name { get; set; }
        public float Duration { get; set; } = 1f;
        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;

        public override void Execute()
        {
            var spineManager = AgainSystem.Instance.SpineManager;
            spineManager.Move(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}