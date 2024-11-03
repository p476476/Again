namespace Again.Runtime.Commands.Image
{
    public class ChangeImageCommand : Command
    {
        public string Name { get; set; }
        public string ImageName { get; set; }

        public override void Execute()
        {
            var imageManager = AgainSystem.Instance.ImageManager;
            imageManager.Change(this);
            AgainSystem.Instance.NextCommand();
        }
    }
}