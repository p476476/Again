using Again.Runtime.Enums;
using DG.Tweening;

namespace Again.Runtime.Commands.Image
{
    public class ShowImageCommand : Command, IScalableCommand
    {
        public string Name { get; set; }

        public string ImageName { get; set; }

        public ShowAnimationType ShowType { get; set; } = ShowAnimationType.Fade;

        public float Duration { get; set; } = 1f;

        public float PosX { get; set; } = 0;

        public float PosY { get; set; } = 0;

        public float ScaleX { get; set; } = 1f;
        
        public float ScaleY { get; set; } = 1f;

        public float NextDuration { get; set; } = -1f;

        public int Order { get; set; } = (int)Enums.Order.Image;


        public override void Execute()
        {
            var imageManager = AgainSystem.Instance.ImageManager;
            if (NextDuration < 0)
            {
                imageManager.Show(this, () => AgainSystem.Instance.NextCommand());
            }
            else
            {
                imageManager.Show(this, () => { });
                DOTween.Sequence().AppendInterval(NextDuration).OnComplete(() => AgainSystem.Instance.NextCommand());
            }
        }
    }
}