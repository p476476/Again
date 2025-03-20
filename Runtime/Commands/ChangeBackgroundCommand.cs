using Again.Runtime.Components.Managers;
using Again.Runtime.Enums;
using UnityEngine;

namespace Again.Runtime.Commands
{
    public class ChangeBackgroundCommand : Command
    {
        public string ImageName { get; set; }
        public Color Color { get; set; }
        public ShowAnimationType ShowType { get; set; } = ShowAnimationType.None;
        public float Duration { get; set; } = 1f;
        
        public override void Execute()
        {
            AgainSystem.Instance.ImageManager.ChangeBackground(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}