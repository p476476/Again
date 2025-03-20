using System.Collections.Generic;
using System.Linq;
using Again.Runtime;
using Again.Runtime.Common;
using Again.Runtime.Components.Views;
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
        public ZzzLog log;

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
            AgainSystem.Instance.EventManager.OnAny<List<string>>(OnEmittedWithParameters);
            AgainSystem.Instance.EventManager.OnAny(OnEmitted);
        }

        private void OnEmitted(string eventName)
        {
            if (eventName == "ShowLog")
                return;
            Debug.Log($"Event: {eventName}");
        }

        private void OnEmittedWithParameters(string eventName, List<string> parameters)
        {
            var parametersString = string.Join(", ", parameters);
            Debug.Log($"Event: {eventName}, Parameters: {parametersString}");
        }

        public void Show()
        {
            gameObject.SetActive(true);
            log.Clear();
            UpdatePages();
        }

        public void Refresh()
        {
            UpdatePages();
        }

        public void UpdateLanguage(int index)
        {
            AgainSystem.Instance.SetLanguage(_languageMap[index]);
        }

        public void BackToMenu()
        {
            AgainSystem.Instance.Stop();
            Show();
        }

        public async void UpdatePages()
        {
            buttonContainer.DestroyChildren();
            AgainSystem.Instance.ReloadTranslation();

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
            AgainSystem.Instance.Execute(page);
        }
    }
}