using System;
using System.Collections.Generic;
using Again.Scripts.Runtime.Commands;
using Again.Scripts.Runtime.Commands.OptionMenu;
using Again.Scripts.Runtime.Enums;
using UnityEngine;

namespace Again.Scripts.Runtime.Components
{
    public class DialogueManager : MonoBehaviour
    {
        public DialogueView dialogueView;
        public OptionMenuView optionMenuView;
        private Language _currentLanguage = Language.Chinese;
        private OptionMenuCommand _currentOptionMenuCommand;
        private SayCommand _currentSayCommand;

        private Dictionary<string, List<string>> _localeDict = new();

        public void Reset()
        {
            dialogueView.Reset();
            optionMenuView.Reset();
        }

        public void ShowDialogue(SayCommand command, Action onComplete = null)
        {
            var text = _GetTextString(command);
            var character = _GetCharacterString(command);

            _currentSayCommand = command;
            dialogueView.ScaleText(command.Scale);
            dialogueView.Show(character, text, onComplete);
            if (command.Voice != null) Debug.Log("Voice: " + command.Voice);
        }

        public void ShowOptionMenu(OptionMenuCommand command, Action<int> onComplete)
        {
            var options = new List<string>();
            foreach (var option in command.Options)
                options.Add(_GetOptionString(option));

            _currentOptionMenuCommand = command;
            optionMenuView.Show(options, onComplete);
        }

        public void Shake(ShakeDialogueCommand command, Action onComplete = null)
        {
            dialogueView.Shake(command.Duration, command.Strength, command.Vibrato, command.Randomness,
                command.Snapping, command.FadeOut, command.ShakeType, onComplete);
        }

        public void SkipDialogue()
        {
            AgainSystem.Instance.NextCommand();
        }

        public void Hide()
        {
            dialogueView.Hide();
        }

        public void SetLanguage(Language language)
        {
            _currentLanguage = language;
            if (_currentSayCommand != null)
                dialogueView.SetCharacterAndText(_GetCharacterString(_currentSayCommand),
                    _GetTextString(_currentSayCommand));
            if (_currentOptionMenuCommand != null)
            {
                var options = new List<string>();
                foreach (var option in _currentOptionMenuCommand.Options)
                    options.Add(_GetOptionString(option));
                optionMenuView.UpdateOptionTexts(options);
            }
        }

        public void SetLocaleDict(Dictionary<string, List<string>> localeDict)
        {
            _localeDict = localeDict;
        }

        private string _GetTextString(SayCommand command)
        {
            var text = command.Text;
            if (command.Key != null)
            {
                if (_localeDict.TryGetValue(command.Key, out var translations))
                    text = translations[(int)_currentLanguage];
                else
                    Debug.LogError("Key not found: " + command.Key);
            }

            return text;
        }

        private string _GetCharacterString(SayCommand command)
        {
            var text = command.Character;

            if (_localeDict.TryGetValue(command.Character, out var translations))
                text = translations[(int)_currentLanguage];
            return text;
        }

        private string _GetOptionString(OptionCommand command)
        {
            var text = command.Text;
            if (command.Key != null)
            {
                if (_localeDict.TryGetValue(command.Key, out var translations))
                    text = translations[(int)_currentLanguage];
                else
                    Debug.LogError("Key not found: " + command.Key);
            }

            return text;
        }
    }
}