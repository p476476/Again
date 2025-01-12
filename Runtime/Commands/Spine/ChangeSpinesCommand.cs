using System.Collections.Generic;

namespace Again.Runtime.Commands.Spine
{
    public class ChangeSpinesCommand : Command
    {
        public string Name { get; set; }
        public List<string> Animations { get; set; }
        public string Skin { get; set; }

        public bool IsLoop { get; set; } = true;

        public override void Execute()
        {
            var spineManager = AgainSystem.Instance.SpineManager;
            spineManager.ChangeSpines(this);
            AgainSystem.Instance.NextCommand();
        }
    }
}