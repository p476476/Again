using TMPro;
using UnityEngine;

namespace Again.Scripts.Runtime.Components
{
    public class TestText : MonoBehaviour
    {
        public TMP_Text text;

        private void Update()
        {
            text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x, text.preferredHeight);
        }
    }
}