using System.Threading.Tasks;

namespace Again.Runtime.Commands
{
    public class WaitCommand : Command
    {
        public float Duration { get; set; } = 1f;

        public override async void Execute()
        {
            var duration = IsSkip ? 0 : (int)(Duration * 1000);
            await Task.Delay(duration);
            AgainSystem.Instance.NextCommand();
        }
    }
}