using UnityEngine;

namespace Again.Scripts.Runtime.Commands
{
    public class ChangeBackgroundCommand : Command
    {
        public string ImageName { get; set; }

        public override void Execute()
        {
            var backgroundImage = AgainSystem.Instance.backgroundImage;
            var sprite = Resources.Load<Sprite>($"Backgrounds/{ImageName}");

            // 如果找到了圖片，就設置為背景
            if (sprite != null)
                backgroundImage.sprite = sprite;
            else
                Debug.LogWarning($"未找到名為 {ImageName} 的背景圖片。");

            AgainSystem.Instance.NextCommand();
        }
    }
}