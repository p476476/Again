namespace Again.Runtime.Commands.Spine
{
    public class ChangeSpineCommand : Command
    {
        public string Name { get; set; }
        public string Animation { get; set; }
        public string Skin { get; set; }

        public bool IsLoop { get; set; } = true;

        public override void Execute()
        {
            var spineManager = AgainSystem.Instance.SpineManager;
            spineManager.Change(this);
            AgainSystem.Instance.NextCommand();
        }
    }
}