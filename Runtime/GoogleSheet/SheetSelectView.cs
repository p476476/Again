using Again.Scripts.Runtime;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Again.Runtime.GoogleSheet
{
    public class SheetSelectView : MonoBehaviour
    {
        public UIContainer animationContainer;
        public RectTransform buttonContainer;
        public GameObject buttonPrefab;
        public TMP_Text titleText;

        public void Start()
        {
            Show();
            titleText.text = AgainSystem.Instance.SheetImporter is GoogleSheetImporter ? "遠端腳本" : "本地腳本";
            AgainSystem.Instance.OnCommandsFinished.AddListener(Show);
        }

        public void Show()
        {
            animationContainer.Show();
            UpdatePages();
        }

        public async void UpdatePages()
        {
            buttonContainer.DestroyChildren();

            var pages = await AgainSystem.Instance.SheetImporter.LoadScripts();
            foreach (var page in pages)
            {
                var button = Instantiate(buttonPrefab, buttonContainer);

                button.SetActive(true);
                button.GetComponentInChildren<Text>().text = page;
                button
                    .GetComponent<UIButton>()
                    .onClickEvent.AddListener(() => OnClickPageButton(page));
            }
        }

        private void OnClickPageButton(string page)
        {
            async void Call()
            {
                AgainSystem.Instance.Execute(page);
                animationContainer.OnHiddenCallback.Event.RemoveAllListeners();
            }

            animationContainer.OnHiddenCallback.Event.AddListener(Call);
            animationContainer.Hide();
        }
    }
}