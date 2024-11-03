namespace Again.Runtime.Commands.Transfer
{
    public class ShowTransferCommand : Command
    {
        public override void Execute()
        {
            var view = AgainSystem.Instance.transferView;
            view.gameObject.SetActive(true);
            AgainSystem.Instance.NextCommand();
        }
    }
}