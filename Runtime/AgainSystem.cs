using System.Collections.Generic;
using System.IO;
using Again.Runtime.Commands;
using Again.Runtime.Components.Interfaces;
using Again.Runtime.Components.Managers;
using Again.Runtime.Enums;
using Again.Runtime.Save.Structs;
using Again.Runtime.ScriptImpoter;
using UnityEngine;
using UnityEngine.Events;

namespace Again.Runtime
{
    public class AgainSystem : MonoBehaviour
    {
        public AgainSystemSetting setting;
        [SerializeField] private Transform uiCanvas;

        private List<Command> _commands;
        private int _currentCommandIndex = -1;
        private bool _isAutoNext;
        private bool _isPause;

        public UnityEvent OnScriptFinished { get; } = new();
        public ITransferView TransferView { get; private set; }
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
            TransferView = Instantiate(setting.transferView, uiCanvas).GetComponent<ITransferView>();
            DialogueManager.Init(uiCanvas, setting);

            if (string.IsNullOrEmpty(setting.googleSheetId))
                SheetImporter = new LocalSheetImporter();
            else
                SheetImporter = new GoogleSheetImporter(setting.googleSheetId, setting.googleApiKey);
            DialogueManager.SetLocaleDict(await SheetImporter.LoadTranslation());
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
            if (_isPause)
            {
                Debug.Log("AgainSystem 暫停中");
                return;
            }

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

        public void Stop()
        {
            CameraManager.Reset();
            SpineManager.Reset();
            ImageManager.Reset();
            DialogueManager.Reset();
            DialogueManager.Hide();

            _currentCommandIndex = -1;
            _commands.Clear();
            _isPause = false;
        }

        public async void ReloadTranslation()
        {
            DialogueManager.SetLocaleDict(await SheetImporter.LoadTranslation());
        }

        public void SetLanguage(Language language)
        {
            DialogueManager.SetLanguage(language);
        }

        public void SetLocaleDict(Dictionary<string, List<string>> dict)
        {
            DialogueManager.SetLocaleDict(dict);
        }

        public void SetPause(bool isPause)
        {
            _isPause = isPause;
        }

        public void SetAutoNext(bool isAutoNext)
        {
            _isAutoNext = isAutoNext;
        }

        public bool GetAutoNext()
        {
            return _isAutoNext;
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