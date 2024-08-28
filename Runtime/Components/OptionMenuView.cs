using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Containers;
using TMPro;
using UnityEngine;

namespace Again.Scripts.Runtime.Components
{
    public class OptionMenuView : MonoBehaviour
    {
        private UIContainer _container;
        private List<UIButton> _optionButtons;

        private void Awake()
        {
            _container = GetComponent<UIContainer>();
            _optionButtons = _container.GetComponentsInChildren<UIButton>().ToList();
            foreach (var button in _optionButtons) button.gameObject.SetActive(false);
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

        public void Show(List<string> options, Action<int> onComplete)
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