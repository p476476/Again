using System;
using System.Collections.Generic;
using Again.Runtime.Common;
using Again.Runtime.Components.Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Again.Runtime.Components.Views
{
    public class OptionMenuView : MonoBehaviour, IOptionMenuView
    {
        private const int MaxOptionCount = 8;
        public GameObject buttonPrefab;
        public Transform buttonContainer;
        private readonly List<Button> _optionButtons = new();

        private void Awake()
        {
            transform.ResetAndHide();

            for (var i = 0; i < MaxOptionCount; i++)
            {
                var button = Instantiate(buttonPrefab, buttonContainer).GetComponent<Button>();
                _optionButtons.Add(button);
            }
        }

        public void Reset()
        {
            gameObject.SetActive(false);
            foreach (var button in _optionButtons)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
                button.GetComponentInChildren<TMP_Text>().text = "";
            }
        }

        public void UpdateOptionTexts(List<string> options)
        {
            for (var i = 0; i < options.Count; i++)
            {
                var optionButton = _optionButtons[i];
                optionButton.GetComponentInChildren<TMP_Text>().text = options[i];
            }
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void ShowOptions(List<string> options, Action<int> onComplete)
        {
            gameObject.SetActive(true);
            for (var i = 0; i < options.Count; i++)
            {
                var optionButton = _optionButtons[i];
                optionButton.gameObject.SetActive(true);
                optionButton.GetComponentInChildren<TMP_Text>().text = options[i];
                var optionId = i;
                optionButton.onClick.AddListener(() =>
                {
                    Hide();
                    onComplete?.Invoke(optionId);
                });
            }
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            foreach (var button in _optionButtons)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }
        }
    }
}