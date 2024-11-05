using System.Collections.Generic;
using System.Linq;
using Again.Runtime;
using Again.Runtime.Common;
using Again.Runtime.Enums;
using Again.Runtime.ScriptImpoter;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Again._Examples.Scripts
{
    public class SheetSelectView : MonoBehaviour
    {
        public Transform animationContainer;
        public RectTransform buttonContainer;
        public GameObject buttonPrefab;
        public TMP_Text titleText;

        private readonly Dictionary<int, Language> _languageMap = new()
        {
            { 0, Language.Chinese },
            { 1, Language.English },
            { 2, Language.Japanese }
        };

        private void Awake()
        {
            transform.localPosition = Vector3.zero;
        }

        public void Start()
        {
            Show();
            titleText.text = AgainSystem.Instance.SheetImporter is GoogleSheetImporter ? "遠端腳本" : "本地腳本";
            AgainSystem.Instance.OnScriptFinished.AddListener(Show);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            UpdatePages();
        }

        public void UpdateLanguage(int index)
        {
            AgainSystem.Instance.SetLanguage(_languageMap[index]);
        }

        public async void UpdatePages()
        {
            buttonContainer.DestroyChildren();

            var pages = await AgainSystem.Instance.SheetImporter.LoadScripts();
            var blackList = new[] { "企劃使用說明", "Config", "CommandList", "SpineList", "Translation" };
            pages.Sort();
            pages.RemoveAll(page => blackList.Contains(page));

            foreach (var page in pages)
            {
                var button = Instantiate(buttonPrefab, buttonContainer);

                button.SetActive(true);
                button.GetComponentInChildren<TMP_Text>().text = page;
                button
                    .GetComponent<Button>()
                    .onClick.AddListener(() => OnClickPageButton(page));
            }
        }

        private void OnClickPageButton(string page)
        {
            animationContainer.gameObject.SetActive(false);
            AgainSystem.Instance.Execute(page);
        }
    }
}