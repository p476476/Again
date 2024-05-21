namespace Again.Scripts.Runtime.Commands.Transfer
{
    public class HideTransferCommand : Command
    {
        public override void Execute()
        {
            var view = AgainSystem.Instance.transferView;
            view.Hide();
            view.OnHiddenCallback.Event.AddListener(OnViewHidden);
        }

        private void OnViewHidden()
        {
            var view = AgainSystem.Instance.transferView;
            view.OnHiddenCallback.Event.RemoveListener(OnViewHidden);
            AgainSystem.Instance.NextCommand();
        }
    }
}