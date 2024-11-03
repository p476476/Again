namespace Again.Scripts.Runtime.Commands.Transfer
{
    public class HideTransferCommand : Command
    {
        public override void Execute()
        {
            var view = AgainSystem.Instance.transferView;
            view.gameObject.SetActive(false);
            AgainSystem.Instance.NextCommand();
        }
    }
}