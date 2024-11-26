using System;
using System.Collections.Generic;
using Again.Runtime.Commands.Dialogue;
using Again.Runtime.Commands.OptionMenu;
using Again.Runtime.Components.Interfaces;
using Again.Runtime.Components.Structs;
using Again.Runtime.Enums;
using UnityEngine;

namespace Again.Runtime.Components.Managers
{
    public class DialogueManager : MonoBehaviour
    {
        private readonly List<DialogueLog> _logs = new();
        private Language _currentLanguage = Language.Chinese;
        private OptionMenuCommand _currentOptionMenuCommand;
        private SayCommand _currentSayCommand;
        private IDialogueView _dialogueView;
        private bool _isDialogueShowing;
        private bool _isOptionMenuShowing;
        private Dictionary<string, List<string>> _localeDict = new();
        private ILogView _logView;
        private IOptionMenuView _optionMenuView;

        public void Reset()
        {
            _dialogueView.Reset();
            _optionMenuView.Reset();
            _logView?.Reset();
        }

        public void Init(Transform uiCanvas, AgainSystemSetting setting)
        {
            var dialogueView = Instantiate(setting.dialogueView, uiCanvas);
            var optionMenuView =
                Instantiate(setting.optionMenuView, uiCanvas);
            _dialogueView = dialogueView.GetComponent<IDialogueView>();
            _optionMenuView = optionMenuView.GetComponent<IOptionMenuView>();

            if (setting.logView != null)
            {
                var logView = Instantiate(setting.logView, uiCanvas);
                if (logView != null) _logView = logView.GetComponent<ILogView>();
            }
        }

        public void SetVisible(bool isVisible)
        {
            if (_isDialogueShowing) _dialogueView.SetVisible(isVisible);
            if (_isOptionMenuShowing) _optionMenuView.SetVisible(isVisible);
        }

        public void ShowDialogue(SayCommand command, Action onComplete = null)
        {
            var callback = new Action(() => { onComplete?.Invoke(); });

            var text = _GetTextString(command);
            var character = _GetCharacterString(command);

            _isDialogueShowing = true;
            _currentSayCommand = command;
            _dialogueView.ScaleText(command.Scale);
            _dialogueView.ShowText(character, text, command.IsSkip, callback);
            _addLog(command);
            if (command.Voice != null) Debug.Log("Voice: " + command.Voice);
        }

        public void ShowOptionMenu(OptionMenuCommand command, Action<int> onComplete)
        {
            var callback = new Action<int>(index =>
            {
                _isOptionMenuShowing = false;
                onComplete(index);
            });

            var options = new List<string>();
            foreach (var option in command.Options)
                options.Add(_GetOptionString(option));

            _isOptionMenuShowing = true;
            _currentOptionMenuCommand = command;
            _optionMenuView.ShowOptions(options, callback);
        }

        public void Shake(ShakeDialogueCommand command, Action onComplete = null)
        {
            _dialogueView.Shake(command.IsSkip ? 0 : command.Duration, command.Strength, command.Vibrato,
                command.Randomness,
                command.Snapping, command.FadeOut, command.ShakeType, onComplete);
        }

        public void Hide()
        {
            _isDialogueShowing = false;
            _dialogueView.Hide();
        }

        public void SetLanguage(Language language)
        {
            _currentLanguage = language;
            if (_currentSayCommand != null)
                _dialogueView.SetCharacterAndText(_GetCharacterString(_currentSayCommand),
                    _GetTextString(_currentSayCommand));
            if (_currentOptionMenuCommand != null)
            {
                var options = new List<string>();
                foreach (var option in _currentOptionMenuCommand.Options)
                    options.Add(_GetOptionString(option));
                _optionMenuView.UpdateOptionTexts(options);
            }

            for (var i = 0; i < _logs.Count; i++)
            {
                var log = _logs[i];
                log.CharacterKey = _GetTranslation(log.CharacterKey);
                log.TextKey = _GetTranslation(log.TextKey);
                _logs[i] = log;
            }

            _logView?.SetLogs(_logs);
        }

        public void SetLocaleDict(Dictionary<string, List<string>> localeDict)
        {
            _localeDict = localeDict;
        }

        public void SetLogs(List<DialogueLog> logs)
        {
            _logs.Clear();
            _logs.AddRange(logs);
            _logView?.SetLogs(logs);
        }

        private void _addLog(SayCommand command)
        {
            var log = new DialogueLog();
            log.CharacterKey = command.Character;
            log.TextKey = command.Text;
            _logs.Add(log);
            _logView?.Add(log);
        }

        private string _GetTextString(SayCommand command)
        {
            var text = command.Text;
            if (command.Key != null)
            {
                if (_localeDict.TryGetValue(command.Key, out var translations))
                    text = translations[(int)_currentLanguage];
                else
                    Debug.Log("Translation key not found: " + command.Key);
            }

            return text;
        }

        private string _GetTranslation(string key)
        {
            if (string.IsNullOrEmpty(key)) return "";
            if (_localeDict.TryGetValue(key, out var translations))
                return translations[(int)_currentLanguage];
            return "";
        }

        private string _GetCharacterString(SayCommand command)
        {
            var text = command.Character;

            if (string.IsNullOrEmpty(text)) return "";

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