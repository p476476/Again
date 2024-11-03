using Again.Runtime.Enums;
using UnityEngine;

namespace Again.Runtime.Commands.Spine
{
    public class ChangeSpineColorCommand : Command
    {
        public string Name { get; set; }
        public Color32 ColorDelta { get; set; } = Color.gray;
        public ChangeColorType ChangeColorType { get; set; } = ChangeColorType.None;

        public override void Execute()
        {
            AgainSystem.Instance.SpineManager.ChangeColor(this);
            AgainSystem.Instance.NextCommand();
        }
    }
}