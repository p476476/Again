using UnityEngine.Events;

namespace Again.Runtime.Components.Interfaces
{
    public interface ITransferView
    {
        public void Show(UnityAction callback);

        public void Hide(UnityAction callback);
    }
}