using System;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using TMPro;
using UnityEngine;

namespace Again.Scripts.Runtime.Components
{
    public class OptionMenuView : MonoBehaviour, IOptionMenuView
    {
        private const int MaxOptionCount = 8;
        public GameObject buttonPrefab;
        public Transform buttonContainer;
        private readonly List<UIButton> _optionButtons = new();
        private UIContainer _container;

        private void Awake()
        {
            _container = GetComponent<UIContainer>();
            for (var i = 0; i < MaxOptionCount; i++)
            {
                var button = Instantiate(buttonPrefab, buttonContainer).GetComponent<UIButton>();
                _optionButtons.Add(button);
            }
        }

        public void Reset()
        {
            _container.InstantHide();
            foreach (var button in _optionButtons)
            {
                button.onClickEvent.RemoveAllListeners();
                button.gameObject.SetActive(false);
                button.GetComponentInChildren<TMP_Text>().text = "";
            }
        }

        public void UpdateOptionTexts(List<string> options)
        {
            if (_container.isHidden) return;
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
            for (var i = 0; i < options.Count; i++)
            {
                var optionButton = _optionButtons[i];
                optionButton.gameObject.SetActive(true);
                optionButton.GetComponentInChildren<TMP_Text>().text = options[i];
                var optionId = i;
                optionButton.GetComponentInChildren<UIButton>().onClickEvent.AddListener(() =>
                {
                    Hide();
                    onComplete?.Invoke(optionId);
                });
            }

            _container.Show();
        }

        private void Hide()
        {
            _container.Hide();
            foreach (var button in _optionButtons)
            {
                button.onClickEvent.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }
        }
    }
}