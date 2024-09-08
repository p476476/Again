using UnityEngine;

namespace Again.Scripts.Runtime.Commands
{
    public class ChangeBackgroundCommand : Command
    {
        public string ImageName { get; set; }

        public override void Execute()
        {
            var sprite = Resources.Load<Sprite>($"Backgrounds/{ImageName}");

            // 如果找到了圖片，就設置為背景
            if (sprite != null)
                AgainSystem.Instance.ImageManager.background.sprite = sprite;
            else
                Debug.LogError($"Line {Id} 未找到背景圖片: {ImageName}");

            AgainSystem.Instance.NextCommand();
        }
    }
}