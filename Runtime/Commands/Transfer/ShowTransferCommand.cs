namespace Again.Runtime.Commands.Transfer
{
    public class ShowTransferCommand : Command
    {
        public override void Execute()
        {
            var view = AgainSystem.Instance.TransferView;
            view.Show(() => { AgainSystem.Instance.NextCommand(); });
        }
    }
}