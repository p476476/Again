using System.Collections.Generic;
using Again.Runtime.GoogleSheet;
using Again.Scripts.Runtime.Commands;
using Again.Scripts.Runtime.Components;
using Again.Scripts.Runtime.Enums;
using Again.Scripts.Runtime.LocalSheet;
using Doozy.Runtime.UIManager.Containers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Again.Scripts.Runtime
{
    public class AgainSystem : MonoBehaviour
    {
        public Image backgroundImage;
        public UIContainer transferView;

        [SerializeField] public UnityEvent OnCommandsFinished = new();
        public string googleSheetId;

        private List<Command> _commands;
        private int _currentCommandIndex = -1;

        public ISheetImporter SheetImporter { get; private set; }
        public DialogueManager DialogueManager { get; private set; }
        public SpineManager SpineManager { get; private set; }
        public ImageManager ImageManager { get; private set; }
        public CameraManager CameraManager { get; private set; }
        public static AgainSystem Instance { get; private set; }

        private async void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            _commands = new List<Command>();
            DialogueManager = GetComponent<DialogueManager>();
            SpineManager = GetComponent<SpineManager>();
            CameraManager = GetComponent<CameraManager>();
            ImageManager = GetComponent<ImageManager>();
            if (googleSheetId != null)
                SheetImporter = new GoogleSheetImporter(googleSheetId);
            else
                SheetImporter = new LocalSheetImporter();
            DialogueManager.SetLocaleDict(await SheetImporter.LoadTransition());
        }

        [ContextMenu("Test")]
        public void Test()
        {
            Execute("test1");
        }

        public async void Execute(string scriptName)
        {
            RunCommands(await SheetImporter.LoadScript(scriptName));
        }

        public void RunCommands(List<Command> commands)
        {
            if (commands.Count == 0)
            {
                Debug.Log("腳本沒有任何指令");
                OnCommandsFinished?.Invoke();
                return;
            }

            _commands = commands;
            _currentCommandIndex = -1;
            NextCommand();
        }

        public void NextCommand()
        {
            _currentCommandIndex++;
            if (_currentCommandIndex < _commands.Count)
            {
                _commands[_currentCommandIndex].Execute();
                return;
            }

            CameraManager.Reset();
            SpineManager.Reset();
            ImageManager.Reset();
            DialogueManager.Reset();
            DialogueManager.Hide();
            OnCommandsFinished?.Invoke();
        }

        public void GoToCommand(Command command)
        {
            _currentCommandIndex = _commands.IndexOf(command) - 1;
            NextCommand();
        }

        public void SetLanguage(int languageId)
        {
            DialogueManager.SetLanguage((Language)languageId);
        }

        public void SetLocaleDict(Dictionary<string, List<string>> dict)
        {
            DialogueManager.SetLocaleDict(dict);
        }
    }
}