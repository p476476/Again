using UnityEngine;

namespace Again.Runtime.Commands.Camera
{
    public class LookAtImageCommand : Command
    {
        public string Name { get; set; }
        public float Duration { get; set; } = 1f;
        public float Scale { get; set; } = 1.5f;
        public float AnchorX { get; set; } = 0.5f;
        public float AnchorY { get; set; } = 0.5f;

        public override void Execute()
        {
            var cameraManager = AgainSystem.Instance.CameraManager;
            var imageManager = AgainSystem.Instance.ImageManager;
            var go = imageManager.GetImageObject(Name);
            if (go == null)
            {
                Debug.LogError("Image Object not found");
                AgainSystem.Instance.NextCommand();
                return;
            }

            cameraManager.LookAtObject(go, IsSkip ? 0 : Duration, Scale, new Vector2(AnchorX, AnchorY),
                () => AgainSystem.Instance.NextCommand());
        }
    }
}