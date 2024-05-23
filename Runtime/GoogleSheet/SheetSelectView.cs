using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.UI;

namespace Again.Runtime.GoogleSheet
{
    public class SheetSelectView : MonoBehaviour
    {
        public UIContainer animationContainer;
        public RectTransform buttonContainer;
        public GoogleSheetImporter importer;
        public GameObject buttonPrefab;

        public void Start()
        {
            Show();
        }

        public void Show()
        {
            animationContainer.Show();
        }

        public void SetPages(List<string> pages)
        {
            foreach (var page in pages)
            {
                var button = Instantiate(buttonPrefab, buttonContainer);

                button.GetComponentInChildren<Text>().text = page;
                button.GetComponent<UIButton>().onClickEvent.AddListener(() => OnClickPageButton(page));
            }
        }

        private void OnClickPageButton(string page)
        {
            async void Call()
            {
                await importer.ImportPage(page);
                animationContainer.OnHiddenCallback.Event.RemoveAllListeners();
            }

            animationContainer.OnHiddenCallback.Event.AddListener(Call);
            animationContainer.Hide();
        }
    }
}