namespace Again.Scripts.Runtime.Commands.Transfer
{
    public class ShowTransferCommand : Command
    {
        public override void Execute()
        {
            var view = AgainSystem.Instance.transferView;
            view.Show();
            view.OnVisibleCallback.Event.AddListener(OnViewVisible);
        }

        private void OnViewVisible()
        {
            var view = AgainSystem.Instance.transferView;
            view.OnVisibleCallback.Event.RemoveListener(OnViewVisible);
            AgainSystem.Instance.NextCommand();
        }
    }
}