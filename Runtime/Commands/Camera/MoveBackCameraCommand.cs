namespace Again.Runtime.Commands.Camera
{
    public class MoveBackCameraCommand : Command
    {
        public float Duration { get; set; } = 1;

        public override void Execute()
        {
            AgainSystem.Instance.CameraManager.MoveBackCamera(this, () => AgainSystem.Instance.NextCommand());
        }
    }
}