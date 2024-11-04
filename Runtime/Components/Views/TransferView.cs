using Again.Runtime.Components.Interfaces;
using UnityEngine;
using UnityEngine.Events;

namespace Again.Runtime.Components.Views
{
    public class TransferView : MonoBehaviour, ITransferView
    {
        public void Show(UnityAction callback)
        {
            gameObject.SetActive(true);
            callback?.Invoke();
        }

        public void Hide(UnityAction callback)
        {
            gameObject.SetActive(false);
            callback?.Invoke();
        }
    }
}