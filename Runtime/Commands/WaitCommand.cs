using System.Threading.Tasks;

namespace Again.Scripts.Runtime.Commands
{
    public class WaitCommand : Command
    {
        public float Duration { get; set; } = 1f;

        public override async void Execute()
        {
            await Task.Delay((int)(Duration * 1000));
            AgainSystem.Instance.NextCommand();
        }
    }
}