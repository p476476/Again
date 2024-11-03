using UnityEngine;

namespace Again.Runtime.Commands.Camera
{
    public class LookAtSpineCommand : Command
    {
        public string Name { get; set; }
        public float Duration { get; set; } = 1f;
        public float Scale { get; set; } = 1.5f;
        public float AnchorX { get; set; } = 0.5f;
        public float AnchorY { get; set; } = 0.5f;

        public override void Execute()
        {
            var cameraManager = AgainSystem.Instance.CameraManager;
            var spineManager = AgainSystem.Instance.SpineManager;
            var go = spineManager.GetSpineObject(Name);
            if (go == null)
            {
                Debug.LogError("Spine not found: " + Name);
                AgainSystem.Instance.NextCommand();
                return;
            }

            cameraManager.LookAtObject(go, Duration, Scale, new Vector2(AnchorX, AnchorY),
                () => AgainSystem.Instance.NextCommand());
        }
    }
}