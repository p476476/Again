using System.Collections.Generic;
using System.IO;
using Again.Runtime.GoogleSheet;
using Again.Scripts.Runtime.Commands;
using Again.Scripts.Runtime.Components;
using Again.Scripts.Runtime.Enums;
using Again.Scripts.Runtime.LocalSheet;
using Again.Scripts.Runtime.SaveData;
using UnityEngine;
using UnityEngine.Events;

namespace Again.Scripts.Runtime
{
    public class AgainSystem : MonoBehaviour
    {
        public Transform transferView;

        [SerializeField] public UnityEvent OnScriptFinished = new();
        public string googleSheetId;

        private List<Command> _commands;
        private int _currentCommandIndex = -1;

        public ISheetImporter SheetImporter { get; private set; }
        public DialogueManager DialogueManager { get; private set; }
        public SpineManager SpineManager { get; private set; }
        public ImageManager ImageManager { get; private set; }
        public CameraManager CameraManager { get; private set; }
        public EventManager EventManager { get; private set; }
        public static AgainSystem Instance { get; private set; }

        private async void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(transform.parent.gameObject);
            _commands = new List<Command>();
            EventManager = new EventManager();
            DialogueManager = GetComponent<DialogueManager>();
            SpineManager = GetComponent<SpineManager>();
            CameraManager = GetComponent<CameraManager>();
            ImageManager = GetComponent<ImageManager>();

            if (string.IsNullOrEmpty(googleSheetId))
                SheetImporter = new LocalSheetImporter();
            else
                SheetImporter = new GoogleSheetImporter(googleSheetId);
            DialogueManager.SetLocaleDict(await SheetImporter.LoadTranslation());
            EventManager.On<List<string>>("test", Test);
        }

        [ContextMenu("Test")]
        public void Test(List<string> list)
        {
            // Execute("test1");
            foreach (var item in list) Debug.Log(item);
        }

        public async void Execute(string scriptName)
        {
            CameraManager.avgCamera.enabled = true;
            RunCommands(await SheetImporter.LoadScript(scriptName));
        }

        public void RunCommands(List<Command> commands)
        {
            if (commands.Count == 0)
            {
                Debug.Log("腳本沒有任何指令");
                OnScriptFinished?.Invoke();
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
            OnScriptFinished?.Invoke();
        }

        public void GoToCommand(Command command)
        {
            _currentCommandIndex = _commands.IndexOf(command) - 1;
            NextCommand();
        }

        public void SetLanguage(Language language)
        {
            DialogueManager.SetLanguage(language);
        }

        public void SetLocaleDict(Dictionary<string, List<string>> dict)
        {
            DialogueManager.SetLocaleDict(dict);
        }

        [ContextMenu("Save")]
        public void Save()
        {
            var saveData = new AgainSystemSaveData
            {
                cameraManagerSaveData = CameraManager.Save(),
                imageManagerSaveData = ImageManager.Save(),
                spineManagerSaveData = SpineManager.Save()
            };
            var str = JsonUtility.ToJson(saveData);
            Debug.Log(str);
            var path = Application.persistentDataPath + "/save.txt";
            File.WriteAllText(path, str);
        }

        [ContextMenu("Load")]
        public void Load()
        {
            var path = Application.persistentDataPath + "/save.txt";
            var str = File.ReadAllText(path);
            ImageManager.Load(str);
        }
    }
}