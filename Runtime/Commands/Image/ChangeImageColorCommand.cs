using Again.Runtime.Enums;
using UnityEngine;

namespace Again.Runtime.Commands.Image
{
    public class ChangeImageColorCommand : Command
    {
        public string Name { get; set; }
        public Color32 ColorDelta { get; set; } = Color.gray;
        public ChangeColorType ChangeColorType { get; set; } = ChangeColorType.None;

        public override void Execute()
        {
            var imageManager = AgainSystem.Instance.ImageManager;
            imageManager.ChangeColor(this);
            AgainSystem.Instance.NextCommand();
        }
    }
}