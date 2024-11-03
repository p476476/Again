using TMPro;
using UnityEngine;

namespace Again.Runtime.Components.Views
{
    public class LogBlock : MonoBehaviour
    {
        public TMP_Text contentText;
        public TMP_Text nameText;
        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            contentText.rectTransform.sizeDelta =
                new Vector2(contentText.rectTransform.sizeDelta.x, contentText.preferredHeight);
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x,
                contentText.preferredHeight + nameText.preferredHeight + 30);
        }

        public void SetName(string speakerName)
        {
            nameText.text = speakerName;
        }

        public void SetContent(string content)
        {
            contentText.text = content;
        }
    }
}